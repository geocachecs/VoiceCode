using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Xml;

namespace ConsoleApp2
{
    class MainGrammar
    {

       static private string[] phonetic_alpha_COMMAND = new string[] {"Alfa", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot",
                "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec",
                "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey",
                "X-ray", "Yankee", "Zulu"};
        static private string[] phonetic_alpha_OUTPUT = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k",
                "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};

        static private string[] shift_phonetic_alpha_COMMAND = new string[] {"Shift Alfa", "Shift Bravo", "Shift Charlie", "Shift Delta",
                "Shift Echo", "Shift Foxtrot", "Shift Golf", "Shift Hotel", "Shift India", "Shift Juliett", "Shift Kilo",
                "Shift Lima", "Shift Mike", "Shift November", "Shift Oscar", "Shift Papa", "Shift Quebec", "Shift Romeo",
                "Shift Sierra", "Shift Tango", "Shift Uniform", "Shift Victor", "Shift Whiskey", "Shift X-ray", "Shift Yankee",
                "Shift Zulu"};
        static private string[] shift_phonetic_alpha_OUTPUT = new string[] { "+a", "+b", "+c", "+d", "+e", "+f", "+g", "+h", "+i",
                "+j", "+k", "+l", "+m", "+n", "+o", "+p", "+q", "+r", "+s", "+t", "+u", "+v", "+w", "+x", "+y", "+z" };

        static private string[] special_char_COMMAND = new string[] { "Left Paren", "Right Paren", "Left Bracket", "Right Bracket",
                "Left Curly", "Right Curly","exclamation","not","percent", "backslash", "forward slash", "slash", "greater than",
                "less than", "tack", "minus", "underscore", "star", "asterisk", "colon", "semicolon",
                "comma", "quote", "doublequote", "singlequote", "equal", "period", "dot" };
        static private string[] special_char_OUTPUT = new string[] { "+9", "+0", "[", "]", "+[", "+]", "+1", "+1", "+5", "\\", "/", "/",
                "+.", "+,", "-", "-" , "+-", "+8", "+8", "+;", ";", ",", "\"", "\"", "\'", "{EQUAL}", ".", "." };

        static private string[] movement_COMMAND = new string[] { "Left", "right", "up", "down", "backspace",
                "delete", "endline","enter", "page up", "page down", "caps lock", "tab", "space", "spacebar", "escape",
                "home"};
        static private string[] movement_OUTPUT = new string[] { "{LEFT}", "{RIGHT}", "{UP}", "{DOWN}", "{BACKSPACE}",
            "{DELETE}", "{END}", "{ENTER}", "{PGUP}", "{PGDN}", "{CAPSLOCK}", "{TAB}", "{SPACE}", "{SPACE}", "{ESC}",
            "{HOME}"};

        static private string[] hold_COMMAND = new string[] { "shift", "control", "alt" };
        static private string[] hold_OUTPUT = new string[] { "+", "^", "%" };

        static private string[] numbers;

        static private string[] Concat(params string[][] lists)
        {
            int totaLength = 0;
            int count = 0; 

            for(int i=0; i<lists.Length; i++)
            {
                totaLength += lists[i].Length;
            }

            string[] output = new string[totaLength];

            for (int i = 0; i < lists.Length; i++)
            {
                for(int j = 0; j<lists[i].Length; i++)
                {
                    output[count] = lists[i][j];
                }
            }

            return output;
        }


        static private Choices CreatePhraseValuePairChoices(string[] command,string[] value)
        {
            Choices c = new Choices();
            if(command.Length != value.Length)
            {
                throw new Exception("MainGrammar.CreatePhraseValuePair(): arguments have different lengths (must be the same)");
            }
            else
            {
                SemanticResultValue[] outArray = new SemanticResultValue[command.Length];
                for (int i=0; i<command.Length; i++)
                {
                    c.Add(new SemanticResultValue(command[i], value[i]));
                }
                
            }
            return c;
        }

        static private string[] CreateStringOfNumbers(int n)
        {
            string[] s = new string[n];
            for(int i=0; i<n; i++)
            {
                s[i] = (i+1).ToString();
            }
            return s;
        }

        static public GrammarBuilder GetBaseGrammar(int n = 1)
        {
            GrammarBuilder gb = new GrammarBuilder();


            numbers = CreateStringOfNumbers(100);

            Choices phonetic_alpha = CreatePhraseValuePairChoices(phonetic_alpha_COMMAND, phonetic_alpha_OUTPUT);
            Choices special_char = CreatePhraseValuePairChoices(special_char_COMMAND, special_char_OUTPUT);
            Choices movement = CreatePhraseValuePairChoices(movement_COMMAND, movement_OUTPUT);
            Choices base_key = new Choices(phonetic_alpha, special_char, movement);

            Choices hold = CreatePhraseValuePairChoices(hold_COMMAND, hold_OUTPUT);
            Choices num = CreatePhraseValuePairChoices(numbers, numbers);

            GrammarBuilder mult = new GrammarBuilder();
            mult.Append(num);
            mult.Append("times");

            gb.Append(new SemanticResultKey($"hold_a_0", hold), 0, 1);
            gb.Append(new SemanticResultKey($"hold_b_0", hold), 0, 1);
            gb.Append(new SemanticResultKey($"hold_c_0", hold), 0, 1);
            gb.Append(new SemanticResultKey($"base_0", base_key)); ///!
            gb.Append(new SemanticResultKey($"mult_0", mult), 0, 1);

            for (int i = 1; i < n; i++)
            {

                gb.Append(new SemanticResultKey($"hold_a_{i}", hold), 0, 1);
                gb.Append(new SemanticResultKey($"hold_b_{i}", hold), 0, 1);
                gb.Append(new SemanticResultKey($"hold_c_{i}", hold), 0, 1);
                gb.Append(new SemanticResultKey($"base_{i}", base_key), 0, 1);
                //gb.Append(new SemanticResultKey($"hold_x_{i}", hold), 0, 1);
                //gb.Append(new SemanticResultKey($"hold_y_{i}", hold), 0, 1);
                //gb.Append(new SemanticResultKey($"hold_z_{i}", hold), 0, 1);
                gb.Append(new SemanticResultKey($"mult_{i}", mult), 0, 1);
            }

            //string srgsdoc = "srgsdoc.xml";
            //XmlWriter writer = XmlWriter.Create(srgsdoc);
            //new SrgsDocument(gb).WriteSrgs(writer);

            //Grammar g = new Grammar(gb);

            return gb;
        }

    }
}
