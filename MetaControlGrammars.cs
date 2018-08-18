using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using System.Xml;


// Syntax: "<MetaControlName> <Parameter1> <Parameter2> <Parameter3>"

public class MetaControlGrammars
{

    //private GrammarBuilder COMMAND_LoadProfile()
    //{
    //    GrammarBuilder gb_tmp = new GrammarBuilder();
    //    Choices c_tmp = new Choices();
    //
    //
    //    foreach (string key in AvailableProfiles.Keys)
    //    {
    //        c_tmp.Add(new SemanticResultValue(key, key));
    //    }
    //    if (AvailableProfiles.Count > 0)
    //    {
    //        gb_tmp.Append("Load profile");
    //        gb_tmp.Append(new SemanticResultKey("LoadProfileName", c_tmp));
    //    }
    //
    //    return gb_tmp;
    //}

    

    private static GrammarBuilder createMetaControl(String metaControlName, params object[] parameters )
    {
        GrammarBuilder gb = new GrammarBuilder();
        Sem
        gb.Append()
    }

    public static GrammarBuilder enableProfile(List<String> profileNames)
    {
        GrammarBuilder gb = new GrammarBuilder();

        Choices profileChoices = new Choices();

        SemanticResultValue semval = new SemanticResultValue("semval_ENABLEPROFILE","Enable Profile");
        gb.Append( new SemanticResultKey("semkey_METACONTROLNAME",semval) );
        foreach(String profile in profileNames)
        {
            profileChoices.Add(new SemanticResultValue("semval_"+profile.ToUpper(), profile ) );
        }
        gb.Append(new SemanticResultKey("semkey_PARAMETER1", profileChoices));
                    //////////////////NOT DONE YET
            
    }
    public static GrammarBuilder disableProfile() { }
    public static GrammarBuilder editProfile() { }
    public static GrammarBuilder createProfile() { }


}
