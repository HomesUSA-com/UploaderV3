namespace Husa.Uploader.Crosscutting.Models
{
    using System;
    using Husa.Extensions.Common;
    using Husa.Uploader.Crosscutting.Enums;

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
            if (!selectedItemId.HasValue)
            {
                return;
            }

            this.SelectedItemID = selectedItemId.Value.ToString();
            this.Status = uploaderStatus.GetEnumDescription();
            this.UploaderStatus = uploaderStatus;
        }

        public Item()
        {
            this.SelectedItemID = string.Empty;
            this.Status = string.Empty;
            this.UploaderStatus = UploaderState.None;
        }

        public string SelectedItemID { get; set; }

        public string Status { get; set; }

        public UploaderState UploaderStatus { get; set; }

        public string StatusInfo => $"SelectedItemID: {this.SelectedItemID} - Status: {this.Status}";
    }
}
