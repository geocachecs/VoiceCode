using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Windows;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using System.Xml;

namespace ConsoleApp2
{

    class Program
    {

        

        static void Test(params object[] args)
        {
            foreach(object s in args)
            {
                if(s is String)
                    Console.WriteLine(s);
                else if(s is List<String>)
                {
                    foreach(String m in (List<String>) s)
                    {
                        Console.WriteLine(m);
                    }
                }
                    Console.WriteLine("something else");
            }
        }

        static void Main(string[] args)
        {
            //Func<int> x2 = MetaControl.Test(88);
            //Console.WriteLine(x2());
            //Console.ReadKey();
            //return;
            List<String> xl = new List<string>();
            String n = "coo";
            xl.Add(n);
            n = "cat";
            Test("cat",xl,"dog");




            GrammarBuilder gb = new GrammarBuilder();
            GrammarBuilder gb2 = new GrammarBuilder();
            Grammar g;
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
            
            //sre.SetInputToDefaultAudioDevice();

            
            gb = MainGrammar.GetBaseGrammar(10);
            
            XmlWriter x = XmlWriter.Create(@"Grammars/test.xml");
            new SrgsDocument(gb).WriteSrgs(x);
            x.Close();
            
            //XmlReader xw = XmlReader.Create(@"Grammars/test.xml");
            //SrgsDocument sr = new SrgsDocument(XmlReader.Create(@"Grammars/test.xml"));
            
            //g = new Grammar(new SrgsDocument(XmlReader.Create(@"Grammars/test.xml")));
            //g = new Grammar(@"Grammars/test.xml");
            g = new Grammar(gb);
            //sre.LoadGrammarAsync(g); ///IMPORTANT

            //sre.BabbleTimeout = TimeSpan.FromSeconds(0);
            //sre.EndSilenceTimeout = TimeSpan.FromSeconds(0);
            //sre.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(0);

            //MetaControl test = new MetaControl(sre);
            //sre.LoadGrammar(test.MetaGrammar())  ;

            //void zz = MetaControl.HandleMetaGrammar(sre);
            //sre.SpeechRecognized += test.HandleMetaGrammar;       // these could be handled added in the grammar classes
            sre.LoadGrammarAsync(new Grammar(MainGrammar.GetMainGrammar()));                                                      //sre.SpeechRecognized += HandleSpeechRecognizedEvent;  // these could be handled added in the grammar classes
            sre.SpeechRecognized += MainGrammar.HandleMainGrammar;
          //  MainGrammar phonetic = new MainGrammar(sre); // I wonder if this will work
                                                         // Also whether it is good design
                                                         // to add the handler in this way
            //sre.RecognizeAsync(RecognizeMode.Multiple);
            System.Threading.Thread.Sleep(2000);
            sre.EmulateRecognizeAsync("Alfa 4 times");
            Console.WriteLine("here");
            while (true)
            {
                
                /*
                if( (result = sre.RecognizeAsync(RecognizeMode.Multiple)) != null)
                {
                    //if (result.Semantics.ContainsKey("basic"))
                    //    Console.WriteLine(result.Semantics["basic"].Value);
                    //if(result.Semantics.ContainsKey("advanced"))
                    //    Console.WriteLine(result.Semantics["advanced"].Value);
                    //SendKeys.SendWait("^+{TAb}");
                    Console.WriteLine(result.Text);
                }
                */
            }

            

            




            while (true)
            {

        //        SendKeys.SendWait(  );
            }
            

        }

    }
}









////////
/*
SpeechRecognitionEngine sre;



Choices command_names = new Choices();
command_names.Add(phonetic_alpha_COMMAND);


GrammarBuilder gb = new GrammarBuilder();
GrammarBuilder gb2 = new GrammarBuilder();
Choices things = new Choices();
things.Add(new SemanticResultValue("computer", "pc"));
things.Add(new SemanticResultValue("lightbulb", "lb"));
things.Add(new SemanticResultValue("pencil", "pl"));
Choices cities = new Choices();
cities.Add(new SemanticResultValue("Chicago", "ORD"));
cities.Add(new SemanticResultValue("Boston", "BOS"));
cities.Add(new SemanticResultValue("Miami", "MIA"));
cities.Add(new SemanticResultValue("Dallas", "DFW"));

gb.Append(new SemanticResultKey("intro", cities));
gb.Append(new SemanticResultKey("exit", things));

Choices pow = new Choices();
pow.Add(new SemanticResultValue("chow chow", "cc"));
pow.Add(new SemanticResultValue(gb,"gb"));

gb2.Append(new SemanticResultKey("BEEP", pow));



//gb.Append("The next word");
//gb.Append("cat");
//gb.Append("END");
Grammar g = new Grammar(gb);
Grammar g2 = new Grammar(gb2);
g.Name = "g";
g2.Name = "g2";
g.Priority = 4;
g2.Priority = 3;
sre = new SpeechRecognitionEngine();
//sre.LoadGrammar(g);
sre.LoadGrammar(g2);
sre.SetInputToDefaultAudioDevice();


string s;
while (true) { 
    RecognitionResult result = sre.Recognize();

    if (result != null)
    {
        if (result.Semantics.ToArray().Length >= 1)
        {
            Console.WriteLine(result.Grammar.Name);
            Console.WriteLine(result.Semantics["BEEP"].Value);
            Console.WriteLine("cat?");
        }

        //Console.WriteLine(result.Words.ToArray()[1].Text);

    }
}
///////////
*/
