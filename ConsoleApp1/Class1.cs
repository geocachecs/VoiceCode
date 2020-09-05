using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


// issues: 
//  recognition is spotty -- don't know why...
//  occasional 'RaceOnRCWCleanup' error - appears to occur when too many grammars are unloaded at once!


namespace ConsoleApp2
{
    class SmartGrammars
    {
        private static string[] keyWords = { "camel", "score", "allcaps", "nocaps" };

        private static char[] delim = { '\x1', '\x2', '\x3', '\x4', '\x5', '\x6', '\x7', '\x8', '\x9', '\xa', '\xb', '\xc', '\xd', '\xe', '\xf',
                    '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1a', '\x1b', '\x1c', '\x1d', '\x1e',
                    '\x1f', '\x20', '\x21', '\x22', '\x23', '\x24', '\x25', '\x26', '\x27', '\x28', '\x29', '\x2a', '\x2b', '\x2c', '\x2d',
                    '\x2e', '\x2f', '\x3a', '\x3b', '\x3c', '\x3d', '\x3e', '\x3f', '\x40', '\x5b', '\x5c', '\x5d', '\x5f', '\x5e', '\x60', '\x7b',
                    '\x7c', '\x7d', '\x7e', '\x7f' };


        private long lastGrammarPurge;
        private long MAX_MILLISECONDS_BETWEEN_PURGE = 5000;
        private int MAX_SMARTWORD_LIST_LENGTH = 1000;
        private string directory;
        private List<string> smartWords;
        public Grammar smartGrammar;

        private readonly object grammarLock = new object();

        public SmartGrammars(string directory)
        {
            this.directory = directory;
            this.smartWords = new List<string>();
            this.smartWords.Add("supercalifragilisticexpialidocious"); // to prevent exceptions when the log file is not available
            this.smartGrammar = null;
            this.lastGrammarPurge = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private void setSmartWords()
        {
            string screenBufText;
            string[] filenames;
            string[] tmpwords;


            filenames = Directory.GetFiles(directory);
            foreach (string filename in filenames)
            {
                try
                {
                    screenBufText = System.IO.File.ReadAllText(filename);
                }
                catch
                {
                    continue;
                }
                    
                //File.Delete(filename);
                tmpwords = screenBufText.Split(SmartGrammars.delim, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in tmpwords)
                {
                    if(word.Length > 2)
                    {
                        this.smartWords.Remove(word.ToLower());
                        this.smartWords.Add(word.ToLower());

                        if (this.smartWords.Count() == this.MAX_SMARTWORD_LIST_LENGTH + 1)
                        {
                            this.smartWords.RemoveAt(0);
                        }
                        else if (this.smartWords.Count() > this.MAX_SMARTWORD_LIST_LENGTH + 1)
                        {
                            throw new Exception("There are more smartWords than should be allowed");
                        }
                    }
                }
            }      
        }

        private void setSmartGrammar() {  

            if(this.smartWords.Count == 0)
            {
                return;
            }

            Choices c_keyWords = new Choices();
            Choices c_smartWords = new Choices();
            GrammarBuilder gb = new GrammarBuilder();
            c_keyWords.Add(SmartGrammars.keyWords);
            c_smartWords.Add(this.smartWords.ToArray());

            gb.Append(c_keyWords);
            gb.Append(c_smartWords, 1,10);

            this.smartGrammar = new Grammar(gb);
            this.smartGrammar.Priority = 1;
            this.smartGrammar.Weight = 1;
            this.smartGrammar.Name = "NEW_SmartGrammar";


        }

        public void HandleSpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            //System.Console.WriteLine(e.Result.Text);

            if (e.Result.Grammar.Name == "OLD_SmartGrammar" || e.Result.Grammar.Name == "SmartGrammar" || e.Result.Grammar.Name == "NEW_SmartGrammar")
            {
                System.Console.WriteLine(e.Result.Grammar.Name);
                String output = "";
                string thisword;
                //{ "camel", "score", "allcaps", "nocaps" }
                string keyword = e.Result.Words[0].Text;
                for (int i = 1; i < e.Result.Words.Count; i++)
                {
                    thisword = e.Result.Words[i].Text;
                    switch (keyword)
                    {
                        case "camel":
                            output += Char.ToUpper(thisword[0]) + thisword.Substring(1);
                            break;
                        case "score":
                            output += thisword + "_";
                            break;
                        case "allcaps":
                            output += thisword.ToUpper();
                            break;
                        case "nocaps":
                            output += thisword.ToLower();
                            break;
                        default:
                            break;
                    }

                }
                if (keyword == "score")
                {
                    output = output.TrimEnd('_');

                }

                System.Console.WriteLine(output);
                SendKeys.SendWait(output);
            }

            // Surreptitiously remove the old grammars
            SpeechRecognitionEngine sre = (SpeechRecognitionEngine)sender;
            RemoveOldGrammars(ref sre);

        }


        public void ConfigureSRE(ref SpeechRecognitionEngine sre)
        {
            this.setSmartWords();
            this.setSmartGrammar();

            if (sre == null)
            {
                sre = new SpeechRecognitionEngine();
            }

            sre.LoadGrammar(this.smartGrammar);
            sre.SpeechRecognized += this.HandleSpeechRecognized;


        }

        public void UpdateSRE(ref SpeechRecognitionEngine sre) {
            this.setSmartWords();
            this.setSmartGrammar();

            sre.LoadGrammar(this.smartGrammar);

            // if exception handler removes a grammar, an exception happens here
            lock (grammarLock)
            {
                foreach (Grammar g in sre.Grammars)
                {
                    if (g.Name == "SmartGrammar")
                    {
                        g.Priority = -128;
                        g.Weight = 0;
                        g.Name = "OLD_SmartGrammar";

                    }
                    else if (g.Name == "NEW_SmartGrammar")
                    {
                        g.Name = "SmartGrammar";
                    }
                }

            }
            
            if( DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond - this.lastGrammarPurge > MAX_MILLISECONDS_BETWEEN_PURGE)
            {
                this.RemoveOldGrammars(ref sre);
            }
        }


        // NOT thread safe
        private void RemoveOldGrammars(ref SpeechRecognitionEngine sre)
        {
            List<Grammar> oldGrammars = new List<Grammar>();

            foreach (Grammar g in sre.Grammars)
            {
                if (g.Name == "OLD_SmartGrammar")
                {
                    oldGrammars.Add(g);

                }
            }
            foreach (Grammar g in oldGrammars)
            {
                lock (grammarLock)
                {
                    try
                    {
                        sre.UnloadGrammar(g);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                
            }

            this.lastGrammarPurge = DateTime.Now.Ticks/ TimeSpan.TicksPerMillisecond;
        }


    }
}
