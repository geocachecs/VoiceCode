using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.IO;
using System.Windows.Forms;


namespace ConsoleApp2
{
    class CustomGrammars : GrammarClass
    {


        static private void addToKeyValuePair(string[] keys, string[] vals, ref IDictionary<string, string> dict)
        {
            if (keys.Length != vals.Length)
            {
                throw new Exception("key and val must be the same length");
            }

            for (int i = 0; i < keys.Length; i++)
            {
                dict.Add(new KeyValuePair<string, string>(keys[i], vals[i]));
            }
        }

        private Dictionary<string, string> customGrammarMap;
        private string grammarName;
        private string filename;
        public Grammar customGrammar;
        private long lastGrammarPurge;
        private long MAX_MILLISECONDS_BETWEEN_PURGE = 30000;
        private int MAX_SMARTWORD_LIST_LENGTH = 1000;

        public CustomGrammars(string filename)
        {
            this.filename = filename;
            this.customGrammarMap = new Dictionary<string, string>();
            this.customGrammar = null;
            this.lastGrammarPurge = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        }

        private void setCustomGrammar()
        {

            string customGrammarFileText = System.IO.File.ReadAllText(filename);
            Dictionary<string, Dictionary<string, string>> raw_mapping = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(customGrammarFileText);

            this.grammarName = raw_mapping.Keys.ToArray()[0];
            this.customGrammarMap = raw_mapping[this.grammarName];

            Choices c_words = new Choices();
            GrammarBuilder gb = new GrammarBuilder();

            c_words.Add(this.customGrammarMap.Keys.ToArray());

            gb.Append(grammarName);
            gb.Append(c_words, 1, 10);
            this.customGrammar = new Grammar(gb);
            this.customGrammar.Name = "CustomGrammar_" + this.grammarName;
            this.customGrammar.Priority = 1;
            this.customGrammar.Weight = 1;
        }

        public override void HandleSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(e.Result.Grammar.Name == "CustomGrammar_"+ this.grammarName)
            {
                System.Console.WriteLine("CustomGrammar_" + this.grammarName);
                System.Console.WriteLine(e.Result.Text);

                for(int i=1; i<e.Result.Words.Count; i++)
                {
                    string wordtext = e.Result.Words[i].Text;
                    SendKeys.SendWait(customGrammarMap[wordtext]);
                }

                
            }
            

        }

        public override void ConfigureSRE(ref SpeechRecognitionEngine sre)
        {
            this.setCustomGrammar();

            if (sre == null)
            {
                sre = new SpeechRecognitionEngine();
            }


            sre.SpeechRecognized += this.HandleSpeechRecognized;
            sre.LoadGrammar(this.customGrammar);


        }

        public override void UpdateSRE(ref SpeechRecognitionEngine sre)
        {
            throw new NotImplementedException();
        }
    }
}
