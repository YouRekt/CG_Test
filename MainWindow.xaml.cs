using CG_Test.Filters;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Test;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BitmapSource? filteredImage;

    private enum FunctionalFilterType
    {
        Invert,
        Brightness_Correction,
        Contrast_Enhancement,
        Gamma_Correction
    };

    private enum ConvolutionFilterType
    {
        Blur,
        Gaussian_Blur,
        Sharpen,
        Edge_Detection,
        Emboss,
        Custom_Filter
    }
    public MainWindow()
    {
        InitializeComponent();
        PopulateFilterMenu();
    }

    private void PopulateFilterMenu()
    {
        foreach (FunctionalFilterType filterType in Enum.GetValues(typeof(FunctionalFilterType)))
        {
            MenuItem filter = new MenuItem
            {
                Header = string.Join(" ", filterType.ToString().Split("_")),
                Tag = filterType,
            };

            filter.Click += FilterMenuItem_Click;
            Functional_Filters.Items.Add(filter);
        }

        foreach (ConvolutionFilterType filterType in Enum.GetValues(typeof(ConvolutionFilterType)))
        {
            MenuItem filter = new MenuItem
            {
                Header = string.Join(" ", filterType.ToString().Split("_")),
                Tag = filterType,
            };

            filter.Click += FilterMenuItem_Click;
            Convolution_Filters.Items.Add(filter);
        }
    }

    private void FilterMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem filter)
        {
            if (filter.Tag is FunctionalFilterType functionalFilterType)
                ApplyFunctionalFilter(functionalFilterType);
            else if (filter.Tag is ConvolutionFilterType convolutionFilterType)
                ApplyConvolutionFilter(convolutionFilterType);
        }
    }


    private void LoadImage_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Select an Image"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
            filteredImage = bitmap;
            // Set the image source
            OriginalImage.Source = bitmap;
            FilteredImage.Source = filteredImage;
        }
    }

    private void ApplyFunctionalFilter(FunctionalFilterType filterType)
    {
        try
        {
            if (filteredImage != null)
            {
                switch (filterType)
                {
                    case FunctionalFilterType.Invert:
                        filteredImage = FunctionalFilters.Invert(filteredImage);
                        break;
                    case FunctionalFilterType.Brightness_Correction:
                        filteredImage = FunctionalFilters.BrightnessCorrection(filteredImage);
                        break;
                    case FunctionalFilterType.Contrast_Enhancement:
                        filteredImage = FunctionalFilters.ContrastEnchancement(filteredImage);
                        break;
                    case FunctionalFilterType.Gamma_Correction:
                        filteredImage = FunctionalFilters.GammaCorrection(filteredImage);
                        break;
                    default:
                        break;
                }
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}");
        }
    }
    private void ApplyConvolutionFilter(ConvolutionFilterType filterType, ConvolutionFilter? custom = null)
    {
        try
        {
            if (filteredImage != null)
            {
                switch (filterType)
                {
                    case ConvolutionFilterType.Blur:
                        filteredImage = ConvolutionFilters.Blur(filteredImage);
                        break;
                    case ConvolutionFilterType.Gaussian_Blur:
                        filteredImage = ConvolutionFilters.GaussianBlur(filteredImage);
                        break;
                    case ConvolutionFilterType.Sharpen:
                        filteredImage = ConvolutionFilters.Sharpen(filteredImage);
                        break;
                    case ConvolutionFilterType.Edge_Detection:
                        filteredImage = ConvolutionFilters.EdgeDetection(filteredImage);
                        break;
                    case ConvolutionFilterType.Emboss:
                        filteredImage = ConvolutionFilters.Emboss(filteredImage);
                        break;
                    case ConvolutionFilterType.Custom_Filter:
                        if(custom == null)
                        {
                            throw new ArgumentNullException("Please provide a custom filter");
                        }
                        filteredImage = ConvolutionFilters.Convolve(filteredImage, custom);
                        break;
                    default:
                        break;
                }
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}");
        }
    }
    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        if (OriginalImage.Source is BitmapSource original)
        {
            filteredImage = original;
            FilteredImage.Source = original;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (filteredImage == null)
        {
            MessageBox.Show("No modified image to save. Please load and apply a filter first.");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
            Title = "Save modified image"
        };


        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            BitmapEncoder encoder;

            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    MessageBox.Show("Unsupported file format.");
                    return;
            }

            encoder.Frames.Add(BitmapFrame.Create(filteredImage));

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}");
            }
        }
    }

    private void EditCustomFilter_Click(object sender, RoutedEventArgs e)
    {
        var editorWindow = new ConvolutionFilterEditor();
        if (editorWindow.ShowDialog() == true)
        {
            MessageBox.Show("Custom filter edited.");
        }
    }
}