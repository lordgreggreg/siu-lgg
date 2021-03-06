﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkinInstaller
{
    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    //First define a new EventArgs class to contain the newly exposed data

    public class BeforeNavigate2EventArgs : CancelEventArgs
    {
        object ppDisp;

        public object PPDisp
        {
            get { return ppDisp; }
            set { ppDisp = value; }
        }
        
        public object url;
        public object flags;
        public object targetFrameName;
        public object postData;
        public object headers;


        public BeforeNavigate2EventArgs(object pDisp,
                           ref object url,
                           ref object flags,
                           ref object targetFrameName,
                           ref object postData,
                           ref object headers,
                           ref bool cancel)
            : base()
        {
            this.ppDisp = pDisp;
            this.Cancel = cancel;

            this.url = url;
            this.flags = flags;
            this.targetFrameName = targetFrameName;
            this.postData = postData;
            this.headers = headers;
        }
    }
    public class NewWindow2EventArgs : CancelEventArgs
    {

        object ppDisp;

        public object PPDisp
        {
            get { return ppDisp; }
            set { ppDisp = value; }
        }


        public NewWindow2EventArgs(ref object ppDisp, ref bool cancel)
            : base()
        {
            this.ppDisp = ppDisp;
            this.Cancel = cancel;
        }
    }

    public class DocumentCompleteEventArgs : EventArgs
    {
        private object ppDisp;
        private object url;

        public object PPDisp
        {
            get { return ppDisp; }
            set { ppDisp = value; }
        }

        public object Url
        {
            get { return url; }
            set { url = value; }
        }

        public DocumentCompleteEventArgs(object ppDisp, object url)
        {
            this.ppDisp = ppDisp;
            this.url = url;

        }
    }

    public class CommandStateChangeEventArgs : EventArgs
    {
        private long command;
        private bool enable;
        public CommandStateChangeEventArgs(long command, ref bool enable)
        {
            this.command = command;
            this.enable = enable;
        }

        public long Command
        {
            get { return command; }
            set { command = value; }
        }

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }
    }


    //Extend the WebBrowser control
    public class ExtendedWebBrowser : WebBrowser
    {
        AxHost.ConnectionPointCookie cookie;
        WebBrowserExtendedEvents events;
        public string[] AdditionalHeaders { get; set; }

        bool renavigating;

        //This method will be called to give you a chance to create your own event sink
        protected override void CreateSink()
        {
            //MAKE SURE TO CALL THE BASE or the normal events won't fire
            base.CreateSink();
            events = new WebBrowserExtendedEvents(this);
            cookie = new AxHost.ConnectionPointCookie(this.ActiveXInstance, events, typeof(DWebBrowserEvents2));
            AdditionalHeaders = new string[] { };
        }

        public object Application
        {
            get
            {
                IWebBrowser2 axWebBrowser = this.ActiveXInstance as IWebBrowser2;
                if (axWebBrowser != null)
                {
                    return axWebBrowser.Application;
                }
                else
                    return null;
            }
        }

        protected override void DetachSink()
        {
            if (null != cookie)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        }

        //This new event will fire for the NewWindow2
        public event EventHandler<NewWindow2EventArgs> NewWindow2;

        protected void OnNewWindow2(ref object ppDisp, ref bool cancel)
        {
            EventHandler<NewWindow2EventArgs> h = NewWindow2;
            NewWindow2EventArgs args = new NewWindow2EventArgs(ref ppDisp, ref cancel);
            if (null != h)
            {
                h(this, args);
            }
            //Pass the cancellation chosen back out to the events
            //Pass the ppDisp chosen back out to the events
            cancel = args.Cancel;
            ppDisp = args.PPDisp;
        }


        public event EventHandler<BeforeNavigate2EventArgs> BeforeNavigate2;
        protected void OnBeforeNavigate2(object pDisp,
                           ref object url,
                           ref object flags,
                           ref object targetFrameName,
                           ref object postData,
                           ref object headers,
                           ref bool cancel)
        {
            EventHandler<BeforeNavigate2EventArgs> h = BeforeNavigate2;
            BeforeNavigate2EventArgs args = new BeforeNavigate2EventArgs(pDisp,
                    ref url,
                    ref flags,
                    ref targetFrameName,
                    ref postData,
                    ref headers,
                    ref cancel);
            if (null != h)
            {
                h(this, args);
            }
            if (!renavigating)
            {
                if (AdditionalHeaders.Length > 0)
                {
                    headers += string.Join("\r\n", AdditionalHeaders) + "\r\n";
                    renavigating = true;
                    cancel = true;
                    Navigate((string)url, (string)targetFrameName, (byte[])postData, (string)headers);
                }
            }
            else
            {
                renavigating = false;
            }
        }


        //This new event will fire for the DocumentComplete
        public event EventHandler<DocumentCompleteEventArgs> DocumentComplete;

        protected void OnDocumentComplete(object ppDisp, object url)
        {
            EventHandler<DocumentCompleteEventArgs> h = DocumentComplete;
            DocumentCompleteEventArgs args = new DocumentCompleteEventArgs(ppDisp, url);
            if (null != h)
            {
                h(this, args);
            }
            //Pass the ppDisp chosen back out to the events
            ppDisp = args.PPDisp;
            //I think url is readonly
        }

        //This new event will fire for the DocumentComplete
        public event EventHandler<CommandStateChangeEventArgs> CommandStateChange;

        protected void OnCommandStateChange(long command, ref bool enable)
        {
            EventHandler<CommandStateChangeEventArgs> h = CommandStateChange;
            CommandStateChangeEventArgs args = new CommandStateChangeEventArgs(command, ref enable);
            if (null != h)
            {
                h(this, args);
            }
        }


        //This class will capture events from the WebBrowser
        public class WebBrowserExtendedEvents : System.Runtime.InteropServices.StandardOleMarshalObject, DWebBrowserEvents2
        {
            ExtendedWebBrowser _Browser;
            public WebBrowserExtendedEvents(ExtendedWebBrowser browser)
            { _Browser = browser; }

            //Implement whichever events you wish
            public void NewWindow2(ref object pDisp, ref bool cancel)
            {
                _Browser.OnNewWindow2(ref pDisp, ref cancel);
            }

            //Implement whichever events you wish
            public void DocumentComplete(object pDisp, ref object url)
            {
                _Browser.OnDocumentComplete(pDisp, url);
            }

            //Implement whichever events you wish
            public void CommandStateChange(long command, bool enable)
            {
                _Browser.OnCommandStateChange(command, ref enable);
            }

            //Implement whichever events you wish
            
            public void BeforeNavigate2(object pDisp,
                                    ref object URL,
                                    ref object Flags,
                                    ref object TargetFrameName,
                                    ref object PostData,
                                    ref object Headers,
                                    ref bool Cancel)
            {
                _Browser.OnBeforeNavigate2(pDisp,
                    ref URL,
                    ref Flags,
                    ref TargetFrameName,
                    ref PostData,
                    ref Headers,
                    ref Cancel);
            }


           


        }
        [ComImport, Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType(TypeLibTypeFlags.FHidden)]
        public interface DWebBrowserEvents2
        {
            [DispId(0x69)]
            void CommandStateChange([In] long command, [In] bool enable);
            [DispId(0x103)]
            void DocumentComplete([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
            [DispId(0xfb)]
            void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object pDisp, [In, Out] ref bool cancel);
            [DispId(250)]
            void BeforeNavigate2([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                                 [In] [MarshalAs(UnmanagedType.Struct)] ref object URL,
                                 [In] [MarshalAs(UnmanagedType.Struct)] ref object Flags,
                                 [In] [MarshalAs(UnmanagedType.Struct)] ref object TargetFrameName,
                                 [In] [MarshalAs(UnmanagedType.Struct)] ref object PostData,
                                 [In] [MarshalAs(UnmanagedType.Struct)] ref object Headers,
                                 [In] [Out] ref bool Cancel);

        }

        [ComImport, Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E"), TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FHidden)]
        public interface IWebBrowser2
        {
            [DispId(100)]
            void GoBack();
            [DispId(0x65)]
            void GoForward();
            [DispId(0x66)]
            void GoHome();
            [DispId(0x67)]
            void GoSearch();
            [DispId(0x68)]
            void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
            [DispId(-550)]
            void Refresh();
            [DispId(0x69)]
            void Refresh2([In] ref object level);
            [DispId(0x6a)]
            void Stop();
            [DispId(200)]
            object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xc9)]
            object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xca)]
            object Container { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xcb)]
            object Document { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xcc)]
            bool TopLevelContainer { get; }
            [DispId(0xcd)]
            string Type { get; }
            [DispId(0xce)]
            int Left { get; set; }
            [DispId(0xcf)]
            int Top { get; set; }
            [DispId(0xd0)]
            int Width { get; set; }
            [DispId(0xd1)]
            int Height { get; set; }
            [DispId(210)]
            string LocationName { get; }
            [DispId(0xd3)]
            string LocationURL { get; }
            [DispId(0xd4)]
            bool Busy { get; }
            [DispId(300)]
            void Quit();
            [DispId(0x12d)]
            void ClientToWindow(out int pcx, out int pcy);
            [DispId(0x12e)]
            void PutProperty([In] string property, [In] object vtValue);
            [DispId(0x12f)]
            object GetProperty([In] string property);
            [DispId(0)]
            string Name { get; }
            [DispId(-515)]
            int HWND { get; }
            [DispId(400)]
            string FullName { get; }
            [DispId(0x191)]
            string Path { get; }
            [DispId(0x192)]
            bool Visible { get; set; }
            [DispId(0x193)]
            bool StatusBar { get; set; }
            [DispId(0x194)]
            string StatusText { get; set; }
            [DispId(0x195)]
            int ToolBar { get; set; }
            [DispId(0x196)]
            bool MenuBar { get; set; }
            [DispId(0x197)]
            bool FullScreen { get; set; }
            [DispId(500)]
            void Navigate2([In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
            [DispId(0x1f7)]
            void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);
            [DispId(-525)]
            WebBrowserReadyState ReadyState { get; }
            [DispId(550)]
            bool Offline { get; set; }
            [DispId(0x227)]
            bool Silent { get; set; }
            [DispId(0x228)]
            bool RegisterAsBrowser { get; set; }
            [DispId(0x229)]
            bool RegisterAsDropTarget { get; set; }
            [DispId(0x22a)]
            bool TheaterMode { get; set; }
            [DispId(0x22b)]
            bool AddressBar { get; set; }
            [DispId(0x22c)]
            bool Resizable { get; set; }
        }
    }
}
