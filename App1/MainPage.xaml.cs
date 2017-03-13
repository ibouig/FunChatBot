using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Syn.Bot;
using Windows.Storage.Streams;
using Windows.Media.SpeechSynthesis;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    static class MediaElementExtensions
    {
        public static async Task PlayStreamAsync(
          this MediaElement mediaElement,
          IRandomAccessStream stream,
          bool disposeStream = true)
        {
            // bool is irrelevant here, just using this to flag task completion.
            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();

            // Note that the MediaElement needs to be in the UI tree for events
            // like MediaEnded to fire.
            RoutedEventHandler endOfPlayHandler = (s, e) =>
            {
                if (disposeStream)
                {
                    stream.Dispose();
                }
                taskCompleted.SetResult(true);
            };
            mediaElement.MediaEnded += endOfPlayHandler;

            mediaElement.SetSource(stream, string.Empty);
            mediaElement.Play();

            await taskCompleted.Task;
            mediaElement.MediaEnded -= endOfPlayHandler;
        }
    }


    public sealed partial class MainPage : Page
    {
        public String help = null;
        public Boolean Sound = false;
        public SynBot Chatbot;
        public MainPage()
        {
            Chatbot = new SynBot();
            Chatbot.PackageManager.LoadFromString(File.ReadAllText("SIMLibouigPackage.simlpk"));
            this.InitializeComponent();
        }

        async Task<IRandomAccessStream> SynthesizeTextToSpeechAsync(string text)
        {
            // Windows.Storage.Streams.IRandomAccessStream
            IRandomAccessStream stream = null;

            // Windows.Media.SpeechSynthesis.SpeechSynthesizer
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                // Windows.Media.SpeechSynthesis.SpeechSynthesisStream
                stream = await synthesizer.SynthesizeTextToStreamAsync(text);
            }

            return (stream);
        }

        async Task SpeakTextAsync(string text, MediaElement mediaElement)
        {
            IRandomAccessStream stream = await this.SynthesizeTextToSpeechAsync(text);

            await mediaElement.PlayStreamAsync(stream, true);
        }



        async private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChatResult result = Chatbot.Chat(InputBox.Text);
            //string.Format("{2}\nUser: {0}\nBot: {1}\n",  , OutputBox.Text);
            OutputBox.Text = result.BotMessage;
            if (Sound) await this.SpeakTextAsync(result.BotMessage, this.uiMediaElement);
            InputBox.Text = string.Empty;
        }
        
        async private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            if (e.Key == Windows.System.VirtualKey.Enter )
            {
                help = InputBox.Text;

                ChatResult result = Chatbot.Chat(help);
                OutputBox.Text = result.BotMessage;
               if(Sound) await this.SpeakTextAsync(result.BotMessage, this.uiMediaElement);
                help = String.Empty;
            }

            help = InputBox.Text;

        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
            InputBox.Text = string.Empty;

        
        }

        private void OutputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputBox.Text = string.Empty;

        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Sound = !(Sound);
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(
                "Created by Said Bouigherdaine - 2016\n" +
                "Email : ibouig@hotmail.fr\n        said.bouigherdaine@gmail.com",
                "FunChatBot");
            
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            var btn = sender as Button;
          
        }
    }
}







/*
 * 
 * using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AIMLbot;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Bot myBot;
        User myUser;
        public MainPage()
        {
            
            string settingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), Path.Combine("config", "Settings.xml"));
            
            //greetingOutput.Text = System.IO.Directory.GetCurrentDirectory();
            myBot = new Bot();
            myBot.loadSettings(settingsPath);
            myUser = new User("consoleUser", myBot);
            System.Xml.XmlDocument s = new System.Xml.XmlDocument();
           

            myBot.isAcceptingUserInput = false;
            myBot.loadAIMLFromFiles();
            myBot.isAcceptingUserInput = true;
            
            this.InitializeComponent();
        }

        private void inputButton_Click(object sender, RoutedEventArgs e)
        {


            Request r = new Request(nameInput.Text, myUser, myBot);
            Result res = myBot.Chat(r);
            greetingOutput.Text = "Hello " + res.Output + "!";
            
            if (input.ToLower() == "quit")
            {
                break;
            }
            else
            {
                Request r = new Request(input, myUser, myBot);
                Result res = myBot.Chat(r);
                Console.WriteLine("Bot: " + res.Output);
            }
            
        }
    }
}



 **/