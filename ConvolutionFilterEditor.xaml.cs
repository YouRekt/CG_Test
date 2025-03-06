using CG_Test.Filters;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CG_Test
{
    public partial class ConvolutionFilterEditor : Window
    {
        private ConvolutionFilterEditorViewModel viewModel;
        public ConvolutionFilter? EditedFilter { get; private set; }
        public ConvolutionFilterEditor(ConvolutionFilter existingFilter)
        {
            InitializeComponent();
            viewModel = new(existingFilter);
            DataContext = viewModel;
        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            viewModel.ApplyChangesToKernel();
            EditedFilter = viewModel.CustomFilter;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ResetFilter();
        }
        private void SaveFilter_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveFilter();
        }
        private void LoadFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Header is string filterName)
            {
                if (DataContext is ConvolutionFilterEditorViewModel viewModel)
                {
                    viewModel.LoadFilter(filterName);
                }
            }
        }
        private void OnAnchorChanged(object sender, RoutedEventArgs e)
        {
            int row, col;
            if (int.TryParse(AnchorX.Text, out col) && int.TryParse(AnchorY.Text, out row))
            {
                viewModel.UpdateAnchor(row, col);
            }
        }
    }
}