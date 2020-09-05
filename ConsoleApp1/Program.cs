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
using Newtonsoft.Json;

namespace ConsoleApp2
{

    class Program
    {

        private static void Usage(string[] args)
        {
            Console.WriteLine("\nUsage:\n\t" + "VoiceCoder" + " <log file folder> <custom grammars folder>\n\n");
            Application.Exit();
        }

        static void HandleTest(object sender, SpeechRecognizedEventArgs e)
        {
            for (var i=0; i< e.Result.Alternates.Count; i++)
            {
                System.Console.WriteLine(e.Result.Alternates[i].Text);
                for (var j = 0; j < e.Result.Alternates[i].Words.Count; j++) {
                    String s = e.Result.Alternates[i].Words[j].Text;
                    var x = e.Result.Alternates[i].Words[j].Confidence;
                    System.Console.WriteLine("Word: {0}\nConfidence: {1}\n", s, x);
                }
                System.Console.WriteLine("===========\n\n");
            }

        }

        static void dumbtest(ref SpeechRecognitionEngine sre) {
            if (sre == null) {
                System.Console.WriteLine("oklol");
            }
        }


        static void Main(string[] args)
        {
            //string json = "{\"key1\":{\"subkey1\":\"value1\"},\"key2\":{\"subkey2\":\"value2\"}}";
            //string json = "{\"key1\":{\"subkey1\":\"value1\"},\"key2\":{\"subkey2\":\"value2\"}}";
            //Dictionary<string, Dictionary<string, string>> x = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            //Console.WriteLine("ok");
            //System.Console.ReadKey();
            //return;

            if(args.Length != 2)
            {
                Console.WriteLine(args.Length);
                Usage(args);
            }
                    

            string text = System.IO.File.ReadAllText(@"test.screenlog");

            SpeechRecognitionEngine sre = null;
            SmartGrammars smartg = new SmartGrammars(@"C:\Users\g\Documents\transcripts");
            smartg.ConfigureSRE(ref sre);
            CustomGrammars cg = new CustomGrammars(@"C:\Users\g\Documents\transcripts\customconfig.txt");

            cg.ConfigureSRE(ref sre);
            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);


            while (true)
            {

                smartg.UpdateSRE(ref sre); // the handler doesn't work when this is commented out??
                System.Threading.Thread.Sleep(2000);

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


//XmlWriter x = XmlWriter.Create(@"Grammars/test.xml");
//new SrgsDocument(gb).WriteSrgs(x);
//x.Close();

//XmlReader xw = XmlReader.Create(@"Grammars/test.xml");
//SrgsDocument sr = new SrgsDocument(XmlReader.Create(@"Grammars/test.xml"));