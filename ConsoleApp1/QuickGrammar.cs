using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace ConsoleApp2
{
    class QuickGrammar
    {
        private Dictionary<String, String> grammar_mappings = new Dictionary<string, string>();
        private String Filename;

        public QuickGrammar(String fn)
        {
            Filename = fn;
        }

        public GrammarBuilder GetQuickGrammar()
        {
            StreamReader reader;
            String s;
            String s2;
            Choices c = new Choices();
            GrammarBuilder gb = new GrammarBuilder();

            reader = new StreamReader(Filename);
            while (reader.Peek() != -1)
            {
                s = reader.ReadLine();
                s = s.Split('#')[0]; // remove comment
                if (s.Length > 0)
                {
                    s2 = s.Split(':')[1];
                    s = s.Split(':')[0];
                    grammar_mappings[s] = s2;
                    c.Add(s);

                }                    
            }

            gb.Append(c);
            return gb;
        }

        public void HandleQuickGrammar(object sender, SpeechRecognizedEventArgs e)
        {
            String s = "";
            if( grammar_mappings.ContainsKey(e.Result.Text))
            {
                s = grammar_mappings[e.Result.Text];
            }

            if (s.Length > 0)
            {
                Console.WriteLine(s);
                SendKeys.SendWait(s);
            }
        } 

    }

    
}
