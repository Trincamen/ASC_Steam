using System;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.DevTools.V120.Network;
using OpenQA.Selenium.DevTools.V85.Runtime;
using OpenQA.Selenium.Support.UI;
using Cookie = OpenQA.Selenium.Cookie;




//configuração do navegador
ChromeOptions options = new ChromeOptions();
    options.AddArgument("--headless=new");



    try
    {
        //essa parte o usuario vai fornecer os cookies do navegador dele logado para inserir no selenium
        Console.WriteLine("Sessionid: ");
        var sessionid = Console.In.ReadLine();
        Console.WriteLine("steamLoginSecure: ");
        var steamLoginSecure = Console.In.ReadLine();
    //ate aqui

    //iniciando navegador
    //se tirar o option funciona abrindo o navegador bom pra debug

        IWebDriver driver = new ChromeDriver (options);

        driver.Navigate().GoToUrl("https://steamcommunity.com");
        Console.Clear();
        //tentando inserir cookies
        Cookie cookie = new Cookie("sessionid", sessionid, "steamcommunity.com", "/", DateTime.Now.AddDays(1));
        Cookie cookie2 = new Cookie("steamLoginSecure", steamLoginSecure, "steamcommunity.com", "/", DateTime.Now.AddDays(1));
        driver.Manage().Cookies.AddCookie(cookie);
        driver.Manage().Cookies.AddCookie(cookie2);
        driver.Navigate().Refresh();
        Console.Clear();
        Thread.Sleep(1000);
        //Aqui o cookies ja estão certo

        //Aqui ele pega o link do perfil atraves do cookies que ja ta logado na conta do usuario
        IWebElement perfil = driver.FindElement(By.CssSelector("#global_actions > a"));
        driver.Navigate().GoToUrl(perfil.GetAttribute("href") + "inventory/#753");
        //

        //Pegando o tanto de cartas para rodar o for ate acabar pega encima do valor da categoria no proprio filtro do site
        IWebElement filtro_cart = driver.FindElement(By.CssSelector("#tabcontent_inventory > div.filter_ctn.inventory_filters > div.filter_tag_button_ctn"));
        filtro_cart.Click();
        Thread.Sleep(2000);
        IWebElement carta_inv = driver.FindElement(By.CssSelector("#tags_76561198125185571_753_0 > div:nth-child(4) > div.econ_tag_filter_container > label > span"));
        string padrao2 = @"\((\d+)\)";
        Match correspondencia2 = Regex.Match(carta_inv.Text, padrao2);
        string total_cartas = correspondencia2.Groups[1].Value;
        Console.WriteLine("Total de:" + total_cartas + " cartas para vender!!");
        int total_cartas_int = Convert.ToInt32(total_cartas);
        filtro_cart.Click();
        Thread.Sleep(5000);
        //fim


        //for roda baseado no numero de cartas que tem no inventario
        for (int i = 0; i < total_cartas_int; i++)
        {
            
            //Para ajustar a informação a ser exibida
            int b = i + 1;
            Console.WriteLine("Esta na:" + b + " carta de " + total_cartas_int);
            //driver.Navigate().Refresh();


            filtro_cart.Click();
            Thread.Sleep(1000);
            IWebElement filtro_cart3 = driver.FindElement(By.CssSelector("#tag_filter_753_0_misc_tradable"));
            filtro_cart3.Click();
            IWebElement filtro_cart4 = driver.FindElement(By.CssSelector("#tags_76561198125185571_753_0 > div:nth-child(4) > div.econ_tag_filter_container"));
            filtro_cart4.Click();


            // IWebElement filtro = driver.FindElement(By.Id("filter_control"));
            //filtro.SendKeys("carta");


            Thread.Sleep(2000);
            IWebElement carta = driver.FindElement(By.CssSelector("#inventory_76561198125185571_753_0 > div > div:nth-child(4)"));
            carta.Click();

            Thread.Sleep(1000);



            //coloquei um try porque esse selector tem duas variação entao se der erro em um ele tenta o outro
            //
            try
            {
                IWebElement vender = driver.FindElement(By.CssSelector("#iteminfo1_item_market_actions > a"));
                IWebElement valor_carta = driver.FindElement(By.CssSelector("#iteminfo1_item_market_actions > div > div:nth-child(2)"));
                vender.Click();

                //padrão para puxar somente o valor da carta como é sempre igual fica facil
                string padrao = @"R\$ (\d+,\d{2})";

                // Procura por correspondências na string usando expressão regular
                Match correspondencia = Regex.Match(valor_carta.Text, padrao);

                // Verifica se uma correspondência foi encontrada e obtém o valor
                if (correspondencia.Success)
                {
                    string valordacarta = correspondencia.Groups[1].Value;
                    
                    double valormenor = Convert.ToDouble(valordacarta) - 0.01;
                     
                    IWebElement campo_preço = driver.FindElement(By.CssSelector("#market_sell_buyercurrency_input"));
                    campo_preço.SendKeys(Convert.ToString(valormenor));

                    Thread.Sleep(1000);
                    IWebElement termo = driver.FindElement(By.CssSelector("#market_sell_dialog_accept_ssa"));
                    if (i == 0)
                {
                    termo.Click();
                }
                    IWebElement vender_button = driver.FindElement(By.CssSelector("#market_sell_dialog_accept"));
                    vender_button.Click();

                    Thread.Sleep(1000);
                    IWebElement button_ok = driver.FindElement(By.CssSelector("#market_sell_dialog_ok > span"));
                    button_ok.Click();
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("valor não encontrado");
                }




            }
            //Outro parte do selector que tem duas opção
            catch
            {
                IWebElement vender2 = driver.FindElement(By.CssSelector("#iteminfo0_item_market_actions > a"));
                IWebElement valor_carta = driver.FindElement(By.CssSelector("#iteminfo0_item_market_actions > div > div:nth-child(2)"));
                vender2.Click();
                string padrao = @"R\$ (\d+,\d{2})";

                Match correspondencia = Regex.Match(valor_carta.Text, padrao);

                if (correspondencia.Success)
                {
                    string valordacarta = correspondencia.Groups[1].Value;

                    double valormenor = Convert.ToDouble(valordacarta) - 0.01;

                    IWebElement campo_preço = driver.FindElement(By.CssSelector("#market_sell_buyercurrency_input"));
                    campo_preço.SendKeys(Convert.ToString(valormenor));

                Thread.Sleep(1000);
                    IWebElement termo = driver.FindElement(By.CssSelector("#market_sell_dialog_accept_ssa"));
                if (i == 0)
                {
                    termo.Click();
                }
                    IWebElement vender_button = driver.FindElement(By.CssSelector("#market_sell_dialog_accept"));
                    vender_button.Click();

                    Thread.Sleep(1000);
                    IWebElement button_ok = driver.FindElement(By.CssSelector("#market_sell_dialog_ok > span"));
                    button_ok.Click();
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("valor nao encontrado");
                }

            }



        }
        driver.Quit();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
Console.WriteLine("Todas as cartas estão anunciadas");