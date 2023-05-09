using Husa.Uploader.Crosscutting.Enums;
using Husa.Uploader.Crosscutting.Extensions;
using System;

namespace Husa.Uploader.Models
{
    public class Item
    {
        public Item(string selectedItemId, UploaderState uploaderStatus)
        {
            if (string.IsNullOrWhiteSpace(selectedItemId))
            {
                throw new ArgumentException($"'{nameof(selectedItemId)}' cannot be null or whitespace.", nameof(selectedItemId));
            }

            this.SelectedItemID = selectedItemId;
            this.Status = uploaderStatus.GetEnumDescription();
            this.UploaderStatus = uploaderStatus;
        }

        public Item(Guid? selectedItemId, UploaderState uploaderStatus)
            : this()
        {
            if (selectedItemId.HasValue)
            {
                this.SelectedItemID = selectedItemId.Value.ToString();
                this.Status = uploaderStatus.GetEnumDescription();
                this.UploaderStatus = uploaderStatus;
            }
        }

        public Item()
        {
            this.SelectedItemID = string.Empty;
            this.Status = string.Empty;
            this.UploaderStatus = UploaderState.None;
        }

        public string SelectedItemID { get; private set; }

        public string Status { get; private set; }

        public UploaderState UploaderStatus { get; private set; }
    }
}
