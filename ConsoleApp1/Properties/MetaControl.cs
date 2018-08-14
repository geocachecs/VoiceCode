using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Recognition;
using System.Xml;



namespace ConsoleApp2
{
    class MetaControl
    {
        private bool editMode;
        private SpeechRecognitionEngine sre;
        private Dictionary<string, Grammar> ActiveProfiles;
        private Dictionary<string, Grammar> AvailableProfiles;
        //private List<string> ActiveProfiles;
        //private List<string> AvailableProfiles;
        private const string ProfileLocation = "Grammars";

        public MetaControl(SpeechRecognitionEngine s) //
        {
            sre = s;
            ActiveProfiles = new Dictionary<string, Grammar>();
            AvailableProfiles = new Dictionary<string, Grammar>();
            //sre.LoadGrammarAsync(MetaGrammar());
            AvailableProfiles.Add("test", new Grammar("Grammars/test.xml"));
            editMode = false;
        }

        static private string[] GetProfilesNames()
        {
            string[] output = { "test" };
            return output;
        }

        private GrammarBuilder COMMAND_LoadProfile()
        {
            GrammarBuilder gb_tmp = new GrammarBuilder();
            Choices c_tmp = new Choices();


            foreach(string key in AvailableProfiles.Keys)
            {
                c_tmp.Add(new SemanticResultValue(key, key));
            }
            if(AvailableProfiles.Count > 0)
            {
                gb_tmp.Append("Load profile");
                gb_tmp.Append(new SemanticResultKey("LoadProfileName", c_tmp));
            }

            return gb_tmp;
        }

        private GrammarBuilder COMMAND_ViewAvailableProfiles()
        {
            
            GrammarBuilder gb = new GrammarBuilder();
            Choices c = new Choices();

            c.Add(new SemanticResultValue("View Available Profiles", "ViewAvailableProfiles"));
            gb.Append(new SemanticResultKey("ViewAvailableProfiles", c));
            return gb;
        }

        private GrammarBuilder COMMAND_ViewActiveProfiles()
        {
            GrammarBuilder gb = new GrammarBuilder();
            Choices c = new Choices();

            c.Add(new SemanticResultValue("View Active Profiles", "ViewActiveProfiles"));
            gb.Append(new SemanticResultKey("ViewActiveProfiles", c));
            return gb;
        }

        private GrammarBuilder COMMAND_RemoveProfile()
        {
            GrammarBuilder gb = new GrammarBuilder();
            Choices c = new Choices();
            foreach(string key in ActiveProfiles.Keys)
            {
                c.Add(new SemanticResultValue(key, key));
            }
            foreach (string key in AvailableProfiles.Keys)
            {
                c.Add(new SemanticResultValue(key, key));
            }
            if (ActiveProfiles.Keys.Count > 0 || AvailableProfiles.Count > 0)
            {
                gb.Append("Remove profile");
                gb.Append(new SemanticResultKey("RemoveProfile", c));
            }
            else
            {
                throw new Exception("COMMAND_RemoveProfile(): No profile names to populate grammar");
            }
            return gb;
        }

        private GrammarBuilder COMMAND_EditProfile()
        {
            GrammarBuilder gb = new GrammarBuilder();
            Choices c = new Choices();

            foreach(string key in ActiveProfiles.Keys)
            {
                c.Add(new SemanticResultValue(key, key));
            }
            foreach (string key in AvailableProfiles.Keys)
            {
                c.Add(new SemanticResultValue(key,key));
            }
            if(ActiveProfiles.Count > 0 || AvailableProfiles.Count > 0)
            {
                gb.Append("Edit profile");
                gb.Append(new SemanticResultKey("EditProfile",c));
            }
            else
            {
                throw new Exception("COMMAND_EditProfile(): No profile names to populate grammar");
            }
            return gb;
        }

        public Grammar MetaGrammar()
        {
            Choices c = new Choices(COMMAND_LoadProfile(), COMMAND_ViewAvailableProfiles(),COMMAND_ViewActiveProfiles(), COMMAND_RemoveProfile(), COMMAND_EditProfile());
            return new Grammar(c);

        }

        public void HandleMetaGrammar(object sender, SpeechRecognizedEventArgs e)
        {
            Grammar result;

            if(editMode == true)
            {
                Console.WriteLine("IN EDIT");
            }

            if (e.Result.Semantics.ContainsKey("LoadProfileName"))
            {
                string profileName = (string) e.Result.Semantics["LoadProfileName"].Value;
                if (AvailableProfiles.ContainsKey(profileName))
                {
                    if (ActiveProfiles.ContainsKey(profileName))
                        throw new Exception("Profile in both ActiveProfiles and AvailableProfiles");
                    Console.WriteLine("Loading profile " + e.Result.Semantics["LoadProfileName"].Value);
                    
                    ActiveProfiles.Add(profileName, AvailableProfiles[profileName]);
                    sre.LoadGrammar(AvailableProfiles[profileName]);//new Grammar(ProfileLocation + "/" + profileName + ".xml")); // value should be profile name
                    AvailableProfiles.Remove(profileName);
                }
            }
            else if (e.Result.Semantics.ContainsKey("ViewAvailableProfiles"))
            {
                Console.WriteLine("\nPROFILES SHOWN HERE");
                foreach (string i in AvailableProfiles.Keys)
                {
                    Console.WriteLine(i);
                }
            }
            else if (e.Result.Semantics.ContainsKey("ViewActiveProfiles"))
            {
                Console.WriteLine("\nACTIVE PROFILES SHOWN HERE");
                foreach(string i in ActiveProfiles.Keys)
                {
                    Console.WriteLine(i);
                }
            }
            else if (e.Result.Semantics.ContainsKey("RemoveProfile"))// ERROR when trying to remove profiles that are not available // add error detection
            {
                Console.WriteLine("REMOVING PROFILE");
                string key = e.Result.Semantics["RemoveProfile"].Value.ToString();
                //ActiveProfiles.TryGetValue(key,out result);
                sre.UnloadGrammar(ActiveProfiles[key]);
                AvailableProfiles.Add(key, ActiveProfiles[key]);
                ActiveProfiles.Remove(key);

            }
            else if (e.Result.Semantics.ContainsKey("EditProfile"))
            {
                Console.WriteLine("EDIT PROFILE");
                editMode = true;
            }
        }

        //static public Action<object, SpeechRecognizedEventArgs> HandleMetaGrammar(SpeechRecognitionEngine sre)
        //{
        //    void _HandleMetaGrammar(object sender, SpeechRecognizedEventArgs e)
        //    {
        //        if (e.Result.Semantics.ContainsKey("LoadProfileName"))
        //        {
        //            sre.LoadGrammar(new Grammar(@"Grammars/" + e.Result.Semantics["LoadProfileName"].Value + ".xml")); // value should be profile name
        //        }
        //        else if (e.Result.Semantics.ContainsKey("ViewAvailableProfiles"))
        //        {
        //            Console.WriteLine("\nPROFILES SHOWN HERn");
        //        }
        //    }
        //
        //    return _HandleMetaGrammar;
        //}

        //public void SetSpeechRecognitionEngine(SpeechRecognitionEngine s)
        //{
        //    sre = s;
        //}

        static private Func<int> Test(int y)
        {
            int x()
            {
                return y;
            }
            return x;
        }


    }
}
