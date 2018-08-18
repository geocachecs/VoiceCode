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

    private static GrammarBuilder createMetaControl(String metaControlName, params object[] parameters )
    {
        GrammarBuilder gb = new GrammarBuilder();
        SemanticResultValue controlName_val = new SemanticResultValue("++semval: "+metaControlName, metaControlName);

        List<object> paramVal = new List<object>(); // Should be a SemanticResultValue OR Choices (with SemanticResultValues in it)

        for(int i=0; i<parameters.Length; i++)
        {

            if(parameters[i] is String)
            {
                paramVal.Add(new SemanticResultValue("++semval: "+parameters[i], parameters[i]));
            } 
            else if(parameters[i] is List<String>)
            {
                Choices theseChoices = new Choices();
                foreach(String str in (List<String>) parameters[i])
                {
                    theseChoices.Add(new SemanticResultValue("++semval: "+str, str)); ///////// if there is an error, check here
                }
                paramVal.Add(theseChoices);
            }
            else
            {
                throw new ArgumentException("createMetaControl() should only take Strings or lists of Strings as parameters (beyond the first argument which must be a string)");
            }
        }

        gb.Append(new SemanticResultKey("METABCONTROLNAME",controlName_val));
        for (int i = 0; i < paramVal.Count; i++)
        {
            if(paramVal[i] is Choices)
            {
                gb.Append(new SemanticResultKey("PARAMETER" + i.ToString(), (Choices)paramVal[i]));
            }
            else if(paramVal[i] is SemanticResultValue)
            {
                gb.Append(new SemanticResultKey("PARAMETER" + i.ToString(), (SemanticResultValue)paramVal[i]));
            }
        }

        return gb;
    }

    public static GrammarBuilder EnableProfile(List<String> profileNames)
    {
        return createMetaControl("Enable Profile", profileNames);
    }
    public static GrammarBuilder disableProfile(List<String> profileNames)
    {
        return createMetaControl("Disable Profile", profileNames);
    }
    public static GrammarBuilder editProfile(List<String> profileNames)
    {
        return createMetaControl("Edit Profile", profileNames);
    }
//    public static GrammarBuilder createProfile() { }


}
