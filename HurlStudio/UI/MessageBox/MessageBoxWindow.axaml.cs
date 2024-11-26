using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HurlStudio.UI.MessageBox.Model;

namespace HurlStudio.UI.MessageBox
{
    public partial class MessageBoxWindow : Window
    {
        private MessageBoxViewModel _viewModel;
        private bool _closedByDefinedAction = false;

        public MessageBoxWindow(MessageBoxViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;

            this.InitializeComponent();
        }

        private void On_MessageBoxWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.CloseMessageBox(MessageBoxResult.Cancel);
                    break;
                case Key.Enter:
                    this.CloseMessageBox(MessageBoxResult.OK);
                    break;
            }
        }

        /// <summary>
        /// Closes the message box with a result depending on the layout type
        /// Input: Result is the Value of the input field
        /// Default: Result is the given MessageBoxResult
        /// </summary>
        /// <param name="result"></param>
        private void CloseMessageBox(MessageBoxResult result)
        {
            _closedByDefinedAction = true;
            if (_viewModel.Layout == MessageBoxLayout.Input)
            {
                this.Close(_viewModel.Value);
            }
            else
            {
                this.Close(result);
            }
        }

        /// <summary>
        /// Focus input box on load if Layout is Input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MessageBoxWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            if (_viewModel.Layout != MessageBoxLayout.Input) return;

            textBoxValue.Focus();
            textBoxValue.SelectionStart = 0;
            textBoxValue.SelectionEnd = _viewModel.Value?.Length ?? 0;
        }

        /// <summary>
        /// Closes the message box with the clicked button definitions'
        /// return value as dialog result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (button.Tag is not MessageBoxResult result) return;

            this.CloseMessageBox(result);
        }
        
        /// <summary>
        /// Close the window again with a dialog result if it was
        /// closed by a defined action (e.g. ESC or Enter)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MessageBoxWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (_closedByDefinedAction) return;

            _closedByDefinedAction = true;
            if (_viewModel.Layout == MessageBoxLayout.Input)
            {
                this.Close(null);
            }
            else
            {
                this.Close(MessageBoxResult.Cancel);
            }
        }
    }
}