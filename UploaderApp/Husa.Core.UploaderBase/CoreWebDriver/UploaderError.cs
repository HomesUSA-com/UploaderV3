using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husa.Core.UploaderBase
{
    public class UploaderError
    {
        string fieldId;
        string fieldLabel;
        string fieldSection;
        string friendlyErrorMessage;
        string errorMessage;
        string fieldInfo;


        public string FieldId { get => fieldId; set => fieldId = value; }
        public string FieldLabel { get => fieldLabel; set => fieldLabel = value; }
        public string FieldSection { get => fieldSection; set => fieldSection = value; }

        public string FriendlyErrorMessage { get => friendlyErrorMessage; set => friendlyErrorMessage = value; }
        public string ErrorMessage { get => errorMessage; set => errorMessage = value; }

        public string FieldInfo { get => fieldInfo; set => fieldInfo = value; }

        public UploaderError(string _fieldId, string _fieldLabel, string _fieldSection, string _friendlyErrorMessage, string _errorMessage)
        {
            FieldId = _fieldId;
            FieldLabel = _fieldLabel;
            FieldSection = _fieldSection;
            FriendlyErrorMessage = _friendlyErrorMessage;
            ErrorMessage = _errorMessage;
            string fi = "Element " + (!String.IsNullOrEmpty(FieldId) ? FieldId : "Unknown");
            if (!String.IsNullOrEmpty(_fieldLabel))
            {
                fi += ". Label: " + _fieldLabel;
            }
            if (!String.IsNullOrEmpty(_fieldSection))
            {
                fi += ", located in the Tab/Section: " + _fieldSection;
            }

            FieldInfo = fi + ". " + _friendlyErrorMessage;
        }
    }
}
