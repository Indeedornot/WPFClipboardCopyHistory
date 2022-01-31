using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using WK.Libraries.SharpClipboardNS;

namespace ClipboardCopyHistory
{
    public sealed class ClipboardListener
    {
        private readonly SharpClipboard _clipboard = new();
        private readonly List<SharpClipboard.ContentTypes> _listenTypes = new();
        public List<ClipboardData> History { get; } = new();

        public void TypeChange(string type)
        {
            var enumType = Enum.Parse<SharpClipboard.ContentTypes>(type);
            if (_listenTypes.Contains(enumType)) _listenTypes.Remove(enumType);
            else _listenTypes.Add(enumType);
        }

        private void Listener(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            var contentType = e.ContentType;
            if (!_listenTypes.Contains(contentType)) return;

            switch (contentType)
            {
                case SharpClipboard.ContentTypes.Files:
                {
                    string[] filesArray = _clipboard.ClipboardFiles.ToArray();
                    string filesString = string.Join(" \n", filesArray);
                    var data = new ClipboardData(filesString, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    History.Add(data);
                    OnClipboardDataReceive(data);
                    break;
                }
                case SharpClipboard.ContentTypes.Text:
                {
                    var data = new ClipboardData(e.Content.ToString(),
                        DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    History.Add(data);
                    OnClipboardDataReceive(data);
                    break;
                }
                case SharpClipboard.ContentTypes.Image:
                {
                    Debug.WriteLine("Image");
                    break;
                }
                case SharpClipboard.ContentTypes.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClipboardDataReceive(ClipboardData content)
        {
            DataReceived?.Invoke(this, content);
        }

        public event EventHandler<ClipboardData> DataReceived;

        public class ClipboardData
        {
            public ClipboardData(string content, string date)
            {
                Content = content;
                Date = date;
            }

            private string Content { get; }

            private string Date { get; }
        }
    }
}
