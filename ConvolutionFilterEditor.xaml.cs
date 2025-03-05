using CG_Test.Filters;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CG_Test
{
    public partial class ConvolutionFilterEditor : Window
    {
        public ConvolutionFilter CustomFilter { get; private set; }

        public ConvolutionFilterEditor(ConvolutionFilter? existingFilter = null)
        {
            InitializeComponent();
            CustomFilter = existingFilter ?? new ConvolutionFilter();
            InitializeCoreComponents();
        }

        private void InitializeCoreComponents()
        { 
            DivisorTextBox.Text = CustomFilter.Divisor.ToString();
            OffsetTextBox.Text = CustomFilter.Offset.ToString();
            AnchorComboBox.SelectedIndex = (int)Math.Min(CustomFilter.Anchor.X, 2);
            UpdateKernelGrid();
        }
        private void KernelHeightCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int val = (int)comboBox.SelectedIndex;
            Trace.WriteLine($"KernelHeight: {val}");
        }

        private void KernelWidthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SetComboBoxSelection(ComboBox comboBox, int value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (int.TryParse(item.Content?.ToString(), out int size) && size == value)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            comboBox.SelectedIndex = 1; // Default to 3x3
        }

        private void UpdateKernelGrid()
        {
            if (!TryGetKernelSize(KernelWidthCombo, out int width) ||
                !TryGetKernelSize(KernelHeightCombo, out int height)) return;

            var kernel = new double[height, width];
            Array.Copy(CustomFilter.Kernel, kernel, Math.Min(CustomFilter.Kernel.Length, kernel.Length));

            CustomFilter.Kernel = kernel;
            KernelGrid.ItemsSource = kernel.Cast<double>().Select(v => new KernelValueWrapper { Value = v }).ToList();
        }

        private bool TryGetKernelSize(ComboBox comboBox, out int size)
        {
            size = comboBox.SelectedItem is ComboBoxItem item &&
                   int.TryParse(item.Content?.ToString(), out int s) ? s : 3;
            return true;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out int width, out int height, out double divisor, out double offset)) return;

            CustomFilter = new ConvolutionFilter
            {
                Kernel = CreateKernelMatrix(width, height),
                Divisor = divisor,
                Offset = offset,
                Anchor = GetAnchorPoint(width, height)
            };

            DialogResult = true;
            Close();
        }

        private bool ValidateInputs(out int width, out int height, out double divisor, out double offset)
        {
            divisor = offset = 0;
            width = height = 3;

            if (!double.TryParse(DivisorTextBox.Text, out divisor) || divisor == 0)
            {
                MessageBox.Show("Please enter a valid non-zero divisor");
                return false;
            }

            if (!double.TryParse(OffsetTextBox.Text, out offset))
            {
                MessageBox.Show("Please enter a valid offset value");
                return false;
            }

            TryGetKernelSize(KernelWidthCombo, out width);
            TryGetKernelSize(KernelHeightCombo, out height);
            return true;
        }

        private double[,] CreateKernelMatrix(int width, int height)
        {
            var kernel = new double[height, width];
            var values = KernelGrid.ItemsSource.Cast<KernelValueWrapper>().Select(w => w.Value).ToArray();

            for (int i = 0; i < Math.Min(values.Length, kernel.Length); i++)
                kernel[i / width, i % width] = values[i];

            return kernel;
        }

        private Point GetAnchorPoint(int width, int height) =>
            (AnchorComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() switch
            {
                "TopLeft" => new Point(0, 0),
                "BottomRight" => new Point(width - 1, height - 1),
                _ => new Point(width / 2, height / 2)
            };

        private void KernelSizeChanged(object sender, SelectionChangedEventArgs e) => UpdateKernelGrid();
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        public class KernelValueWrapper { public double Value { get; set; } }

    }
}