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
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;

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


        //private static readonly object smartGrammarLock = new object();
        //static private void updateAllSmartGrammars(string directorypath, ref SpeechRecognitionEngine sre)
        //{

        //}


        private static List<SmartGrammars> smartGrammarList;
        private static string[] allScreenBufferFilenames;
       



        static void Main(string[] args)
        {

            if (args.Length != 2)
            {
                Usage(args);
            }
                    
            SpeechRecognitionEngine sre = null;

            SmartGrammars sg = new SmartGrammars(args[0]);
            sg.ConfigureSRE(ref sre);



            List<CustomGrammars> customGrammarList = new List<CustomGrammars>();
            string[] allCustomFilenames = Directory.GetFiles(args[1]);
            foreach(string filename in allCustomFilenames)
            {
                try
                {
                    CustomGrammars cg = new CustomGrammars(filename);
                    customGrammarList.Add(cg);
                    cg.ConfigureSRE(ref sre);
                }
                catch
                {
                    ; // do nothing
                }
                
            }

            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);

            
            while (true)
            {



                sg.UpdateSRE(ref sre);


                System.Threading.Thread.Sleep(1000);

            }



        }

    }
}


