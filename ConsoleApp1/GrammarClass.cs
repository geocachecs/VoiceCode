using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;


namespace ConsoleApp2
{
    abstract class GrammarClass
    {
        protected static readonly object grammarLock = new object();

        abstract public void HandleSpeechRecognized(object sender, SpeechRecognizedEventArgs e);
        abstract public void ConfigureSRE(ref SpeechRecognitionEngine sre);
        abstract public void UpdateSRE(ref SpeechRecognitionEngine sre);

    }
}
