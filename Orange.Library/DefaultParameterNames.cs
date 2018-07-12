using System.Collections.Generic;
using static Orange.Library.Runtime;

namespace Orange.Library
{
   public class DefaultParameterNames
   {
      public const string VAR_VALUE = "_";
      public const string VAR_VALUE1 = "$x";
      public const string VAR_VALUE2 = "$y";
      public const string VAR_KEY = "$k";
      public const string VAR_INDEX = "$i";
      public const string VAR_LINE = "$li";
      public const string VAR_NUMBER = "$lno";
      public const string VAR_ARRAY = "$arr";
      public const string VAR_ITER = "$i";
      public const string VAR_POSITION = "$p";
      public const string VAR_LENGTH = "$l";
      public const string VAR_ERROR = "$e";
      public const string VAR_READER = "$rdr";
      public const string VAR_WRITER = "$wrt";
      public const string VAR_GROUP = "$g";

      public DefaultParameterNames(bool firstPush = false)
      {
         PopAtEnd = false;
         if (!firstPush && State.DefaultParameterNames.IsUpperLevel)
         {
            var upperLevelNames = State.DefaultParameterNames;
            ValueVariable = upperLevelNames.ValueVariable;
            ValueVariable1 = upperLevelNames.ValueVariable1;
            ValueVariable2 = upperLevelNames.ValueVariable2;
            KeyVariable = upperLevelNames.KeyVariable;
            IndexVariable = upperLevelNames.IndexVariable;
            LineVariable = upperLevelNames.LineVariable;
            NumberVariable = upperLevelNames.NumberVariable;
            ArrayVariable = upperLevelNames.ArrayVariable;
            IterVariable = upperLevelNames.IterVariable;
            PositionVariable = upperLevelNames.PositionVariable;
            LengthVariable = upperLevelNames.LengthVariable;
            ErrorVariable = upperLevelNames.ErrorVariable;
            ReaderVariable = upperLevelNames.ReaderVariable;
            WriterVariable = upperLevelNames.WriterVariable;
            GroupVariable = upperLevelNames.GroupVariable;
            UnpackedVariables = new List<string>(upperLevelNames.UnpackedVariables);
         }
         else
         {
            ValueVariable = VAR_VALUE;
            ValueVariable1 = VAR_VALUE1;
            ValueVariable2 = VAR_VALUE2;
            KeyVariable = VAR_KEY;
            IndexVariable = VAR_INDEX;
            LineVariable = VAR_LINE;
            NumberVariable = VAR_NUMBER;
            ArrayVariable = VAR_ARRAY;
            IterVariable = VAR_ITER;
            PositionVariable = VAR_POSITION;
            LengthVariable = VAR_LENGTH;
            ErrorVariable = VAR_ERROR;
            ReaderVariable = VAR_READER;
            WriterVariable = VAR_WRITER;
            GroupVariable = VAR_GROUP;
            UnpackedVariables = new List<string>();
         }
      }

      public bool IsUpperLevel { get; set; }

      public bool PopAtEnd { get; set; }

      public string ValueVariable { get; set; }

      public string ValueVariable1 { get; set; }

      public string ValueVariable2 { get; set; }

      public string KeyVariable { get; set; }

      public string IndexVariable { get; set; }

      public string LineVariable { get; set; }

      public string NumberVariable { get; set; }

      public string ArrayVariable { get; set; }

      public string IterVariable { get; set; }

      public string PositionVariable { get; set; }

      public string LengthVariable { get; set; }

      public string ErrorVariable { get; set; }

      public string ReaderVariable { get; set; }

      public string WriterVariable { get; set; }

      public string GroupVariable { get; set; }

      public List<string> UnpackedVariables { get; set; }

      public DefaultParameterNames Clone()
      {
         var upperLevelNames = State.DefaultParameterNames;
         return new DefaultParameterNames
         {
            ValueVariable = upperLevelNames.ValueVariable,
            ValueVariable1 = upperLevelNames.ValueVariable1,
            ValueVariable2 = upperLevelNames.ValueVariable2,
            KeyVariable = upperLevelNames.KeyVariable,
            IndexVariable = upperLevelNames.IndexVariable,
            LineVariable = upperLevelNames.LineVariable,
            NumberVariable = upperLevelNames.NumberVariable,
            ArrayVariable = upperLevelNames.ArrayVariable,
            IterVariable = upperLevelNames.IterVariable,
            PositionVariable = upperLevelNames.PositionVariable,
            LengthVariable = upperLevelNames.LengthVariable,
            ErrorVariable = upperLevelNames.ErrorVariable,
            ReaderVariable = upperLevelNames.ReaderVariable,
            WriterVariable = upperLevelNames.WriterVariable,
            GroupVariable = upperLevelNames.GroupVariable,
            UnpackedVariables = new List<string>(upperLevelNames.UnpackedVariables)
         };
      }
   }
}