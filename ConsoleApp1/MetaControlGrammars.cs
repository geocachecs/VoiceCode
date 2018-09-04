using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using System.IO;
using System.Xml;
using ConsoleApp2;


// Syntax: "<METACONTROLNAME> <PARAMETER1> <PARAMETER2> <PARAMETER3>"

public class MetaControlGrammars
{

    private SpeechRecognitionEngine sre;
    private String directory;

    public MetaControlGrammars(SpeechRecognitionEngine sre, String directory)
    {
        this.sre = sre;
        this.directory = directory;
    }

    private static GrammarBuilder createMetaControlGB(String metaControlName, params object[] parameters )
    {
        GrammarBuilder gb = new GrammarBuilder();
        SemanticResultValue controlName_val = new SemanticResultValue(/*phrase*/metaControlName, /*value*/metaControlName);

        List<object> paramVal = new List<object>(); // Should be a SemanticResultValue OR Choices (with SemanticResultValues in it)

        for(int i=0; i<parameters.Length; i++)
        {

            if(parameters[i] is String)
            {
                paramVal.Add(new SemanticResultValue( /*phrase*/(String) parameters[i], /*value*/parameters[i] ));
            } 
            else if(parameters[i] is List<String>)
            {
                Choices theseChoices = new Choices();
                foreach(String str in (List<String>) parameters[i])
                {
                    theseChoices.Add(new SemanticResultValue(/*phrase*/str,/*value*/str)); ///////// if there is an error, check here
                }
                paramVal.Add(theseChoices); 
            }
            else
            {
                throw new ArgumentException("createMetaControl() should only take Strings or lists of Strings as parameters (beyond the first argument which must be a string)");
            }
        }

        gb.Append(new SemanticResultKey("METACONTROLNAME",controlName_val));
        for (int i = 0; i < paramVal.Count; i++)
        {
            if(paramVal[i] is Choices)
            {
                gb.Append(new SemanticResultKey("PARAMETER" + (i+1).ToString(), (Choices)paramVal[i]));
            }
            else if(paramVal[i] is SemanticResultValue)
            {
                gb.Append(new SemanticResultKey("PARAMETER" + (i+1).ToString(), (SemanticResultValue)paramVal[i]));
            }
        }

        return gb;
    }

    private static GrammarBuilder EnableProfile(List<String> profileNames)
    {
        return createMetaControlGB("Enable Profile", profileNames);
    }
    private static GrammarBuilder DisableProfile(List<String> profileNames)
    {
        return createMetaControlGB("Disable Profile", profileNames);
    }
    private static GrammarBuilder EditProfile(List<String> profileNames)
    {
        return createMetaControlGB("Edit Profile", profileNames);
    }
    //    private static GrammarBuilder createProfile() { }

    private void Handle_metacontrol_all(object sender, SpeechRecognizedEventArgs e)
    {
        // Must make sure a metacontrol grammar is being used
        if (e.Result.Semantics.ContainsKey("METACONTROLNAME"))
        {
            String MetaControlName = e.Result.Semantics["METACONTROLNAME"].Value.ToString();
            if (MetaControlName == "Enable Profile")
            {
                String grammarName = e.Result.Semantics["PARAMETER1"].Value.ToString();
                QuickGrammar qg = new QuickGrammar(directory + "\\" + grammarName + ".txt");
                sre.LoadGrammarAsync(new Grammar(qg.GetQuickGrammar()));
                sre.SpeechRecognized += qg.HandleQuickGrammar;
            }


        }
    }

    public void ConfigureSRE(ref SpeechRecognitionEngine sre)
    {
        List<String> profileNames = new List<string>();
        FileInfo[] Files = new DirectoryInfo(@directory).GetFiles("*.txt"); //Getting Text files
        foreach (FileInfo file in Files)
        {
            profileNames.Add(file.Name.Split('.')[0]);
        }

        GrammarBuilder metacontrolgb_all = new GrammarBuilder();
        Choices c = new Choices();
        c.Add(EnableProfile(profileNames));
        c.Add(DisableProfile(profileNames));
        c.Add(EditProfile(profileNames));
        metacontrolgb_all.Append(c);

        sre.LoadGrammarAsync(new Grammar(metacontrolgb_all));
        sre.SpeechRecognized += Handle_metacontrol_all;

        //sre.LoadGrammarAsync(new Grammar(EnableProfile(profileNames)));
        ////sre.SpeechRecognized += Handle_EnableProfile;
        //sre.LoadGrammarAsync(new Grammar(DisableProfile(profileNames)));
        ////sre.SpeechRecognized += Handle_DisableProfile;
        //sre.LoadGrammarAsync(new Grammar(EditProfile(profileNames)));
        ////sre.SpeechRecognized += Handle_EditProfile;


    }

}
