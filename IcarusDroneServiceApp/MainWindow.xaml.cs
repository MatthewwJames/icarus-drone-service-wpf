using IcarusDroneServiceWPF;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace IcarusDroneServiceApp
{
    public partial class MainWindow : Window
    {
        // 6.2 Global List<T> of type Drone called FinishedList
        private readonly List<Drone> FinishedList = new List<Drone>();

        // 6.3 Global Queue<T> of type Drone called RegularService
        private readonly Queue<Drone> RegularService = new Queue<Drone>();

        // 6.4 Global Queue<T> of type Drone called ExpressService
        private readonly Queue<Drone> ExpressService = new Queue<Drone>();

        public MainWindow()
        {
            InitializeComponent();

            // Applies the Service Cost validation to pasted text as well as typed text
            DataObject.AddPastingHandler(
                txtServiceCost,
                ServiceCost_Pasting
            );
        }

        // 6.5 Adds a new service item to the correct Queue based on selected priority
        // 6.6 Adds 15% to the service cost before adding an Express service item
        // 6.7 Calls GetServicePriority before adding the item to a Queue
        // 6.11 Calls IncrementServiceTag before completing the add process
        // 6.17 Calls ClearTextboxes after a service item has been added
        private void AddNewItem(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                txtStatus.Text = "Client name is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDroneModel.Text))
            {
                txtStatus.Text = "Drone model is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtServiceProblem.Text))
            {
                txtStatus.Text = "Service problem is required.";
                return;
            }

            if (!ValidateServiceCost(out double serviceCost))
            {
                return;
            }
            // changed the Service Tag to be a numeric control value
            if (!numServiceTag.Value.HasValue)
            {
                txtStatus.Text = "Service tag is required.";
                return;
            }

            int serviceTag = numServiceTag.Value.Value;

            string priority = GetServicePriority();

            if (priority == "Express")
            {
                serviceCost = serviceCost * 1.15;
            }

            Drone newDrone = new Drone(
                txtClientName.Text,
                txtDroneModel.Text,
                txtServiceProblem.Text,
                serviceCost,
                serviceTag
            );

            IncrementServiceTag();

            if (priority == "Regular")
            {
                RegularService.Enqueue(newDrone);
                DisplayRegularService();
                txtStatus.Text = "New regular service item added.";
            }
            else
            {
                ExpressService.Enqueue(newDrone);
                DisplayExpressService();
                txtStatus.Text = "New express service item added with 15% charge.";
            }

            ClearTextboxes();
        }

        // 6.7 Returns the value of the selected priority radio button
        private string GetServicePriority()
        {
            if (radExpress.IsChecked == true)
            {
                return "Express";
            }

            return "Regular";
        }

        // 6.10 Performs the final validation of the Service Cost
        // The value must be a valid non-negative double with no more than
        // two digits after the decimal point
        private bool ValidateServiceCost(out double serviceCost)
        {
            serviceCost = 0;

            string serviceCostText = txtServiceCost.Text.Trim();

            if (string.IsNullOrWhiteSpace(serviceCostText))
            {
                txtStatus.Text = "Service cost is required.";
                return false;
            }

            // Final pattern requires at least one digit.
            // It permits a whole number or one/two decimal places.
            // "^" Start Anchor which forces the match to start the very beginning of the text, so that way no text can be before the number.
            // "/d+" Digits, makes it require one or more digits (0-9) for the whole number part (etc 5, 100, 0)
            // "(....)" The "?" at the end makes everything inside the parentheses optional, so that way it can allow whole numbers without decimals (etc 50)
            // "\." matches the actual decimal point, and the backslash is required because a raw dot would mean "any character" in regex
            // "\d{1,2} is the decimal places, which restricts the number after the decimal point to exactly one or two digits (etc .5 or 50)
            // "$" End Anchor which forces the match to end at the very end of the text, so that way no text can be after the number.

            // passes: 99, 99.9, 99.90, 125.50, 0.25
            // fails: 99.999, 99. 12..50, hello, $50

            Regex completeCostPattern =
                new Regex(@"^\d+(\.\d{1,2})?$");

            if (!completeCostPattern.IsMatch(serviceCostText))
            {
                txtStatus.Text =
                    "Service cost must be a number with no more than two decimal places.";

                txtServiceCost.Focus();
                txtServiceCost.SelectAll();

                return false;
            }

            bool validDouble = double.TryParse(
                serviceCostText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out serviceCost
            );

            if (!validDouble)
            {
                txtStatus.Text = "Service cost must be a valid number.";

                txtServiceCost.Focus();
                txtServiceCost.SelectAll();

                return false;
            }

            if (serviceCost < 0)
            {
                txtStatus.Text = "Service cost cannot be negative.";

                txtServiceCost.Focus();
                txtServiceCost.SelectAll();

                return false;
            }

            // Ensures values such as 99.9 are displayed as 99.90
            txtServiceCost.Text =
                serviceCost.ToString("0.00", CultureInfo.InvariantCulture);

            return true;
        }

        // 6.10 Custom method used while the user is editing Service Cost
        // It allows an empty textbox while editing, digits, one decimal point
        // and a maximum of two digits after the decimal point
        private bool IsValidServiceCostInput(string text)
        {
            Regex inputPattern =
                new Regex(@"^\d*(\.\d{0,2})?$");

            return inputPattern.IsMatch(text);
        }

        // 6.11 Increments the numeric Service Tag control by 10
        // The value cannot exceed the maximum of 900
        private void IncrementServiceTag()
        {
            int currentTag = numServiceTag.Value ?? 100;

            numServiceTag.Value = Math.Min(currentTag + 10, 900);
        }

        // 6.8 Displays all elements in the RegularService Queue using a ListView.
        private void DisplayRegularService()
        {
            lvRegularQueue.ItemsSource = null;
            lvRegularQueue.ItemsSource = RegularService.ToList();
        }

        // 6.9 Displays all elements in the ExpressService Queue using a ListView.
        private void DisplayExpressService()
        {
            lvExpressQueue.ItemsSource = null;
            lvExpressQueue.ItemsSource = ExpressService.ToList();
        }

        // Displays all completed service items in the finished ListBox.
        private void DisplayFinishedList()
        {
            lstFinishedItems.ItemsSource = null;
            lstFinishedItems.ItemsSource = FinishedList.Select(drone => drone.Display()).ToList();
        }

        // 6.14 Dequeues the next Regular service item and adds it to the FinishedList.
        private void CompleteRegular_Click(object sender, RoutedEventArgs e)
        {
            if (RegularService.Count == 0)
            {
                txtStatus.Text = "No regular service items to complete.";
                return;
            }

            Drone completedDrone = RegularService.Dequeue();
            FinishedList.Add(completedDrone);

            DisplayRegularService();
            DisplayFinishedList();

            txtStatus.Text = "Regular service item completed and moved to finished list.";
        }

        // 6.15 Dequeues the next Express service item and adds it to the FinishedList.
        private void CompleteExpress_Click(object sender, RoutedEventArgs e)
        {
            if (ExpressService.Count == 0)
            {
                txtStatus.Text = "No express service items to complete.";
                return;
            }

            Drone completedDrone = ExpressService.Dequeue();
            FinishedList.Add(completedDrone);

            DisplayExpressService();
            DisplayFinishedList();

            txtStatus.Text = "Express service item completed and moved to finished list.";
        }

        // 6.16 Deletes the selected finished service item from the ListBox and FinishedList.
        private void FinishedItems_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = lstFinishedItems.SelectedIndex;

            if (selectedIndex < 0)
            {
                txtStatus.Text = "Select a finished item to remove.";
                return;
            }

            FinishedList.RemoveAt(selectedIndex);
            DisplayFinishedList();

            txtStatus.Text = "Finished item removed.";
        }

        // 6.12 Displays the selected Regular service item's Client Name and Service Problem in the textboxes.
        private void RegularQueue_Click(object sender, MouseButtonEventArgs e)
        {
            if (lvRegularQueue.SelectedItem is Drone selectedDrone)
            {
                txtClientName.Text = selectedDrone.GetClientName();
                txtServiceProblem.Text = selectedDrone.GetServiceProblem();
                txtStatus.Text = "Regular service item selected.";
            }
        }

        // 6.13 Displays the selected Express service item's Client Name and Service Problem in the textboxes.
        private void ExpressQueue_Click(object sender, MouseButtonEventArgs e)
        {
            if (lvExpressQueue.SelectedItem is Drone selectedDrone)
            {
                txtClientName.Text = selectedDrone.GetClientName();
                txtServiceProblem.Text = selectedDrone.GetServiceProblem();
                txtStatus.Text = "Express service item selected.";
            }
        }

        // Clears the input textboxes when the Clear Inputs button is clicked.
        private void ClearInputs_Click(object sender, RoutedEventArgs e)
        {
            ClearTextboxes();
            txtStatus.Text = "Inputs cleared.";
        }

        // 6.10 Prevents invalid characters and limits the Service Cost textbox to two decimal places.
        // 6.10 Checks each keyboard character before it appears
        // in the Service Cost textbox.
        private void ServiceCost_PreviewTextInput(
            object sender,
            TextCompositionEventArgs e
        )
        {
            string currentText = txtServiceCost.Text;

            int selectionStart = txtServiceCost.SelectionStart;
            int selectionLength = txtServiceCost.SelectionLength;

            // Remove highlighted text first, then insert the new character.
            string proposedText = currentText
                .Remove(selectionStart, selectionLength)
                .Insert(selectionStart, e.Text);

            // Handled = true blocks the character.
            e.Handled = !IsValidServiceCostInput(proposedText);
        }

        // 6.10 Prevents invalid text from being pasted into Service Cost.
        private void ServiceCost_Pasting(
            object sender,
            DataObjectPastingEventArgs e
        )
        {
            if (sender is not TextBox textBox)
            {
                e.CancelCommand();
                return;
            }

            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string pastedText =
                e.DataObject.GetData(DataFormats.Text) as string ?? "";

            string proposedText = textBox.Text
                .Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, pastedText);

            if (!IsValidServiceCostInput(proposedText))
            {
                e.CancelCommand();

                txtStatus.Text =
                    "Service cost can only contain a number with up to two decimal places.";
            }
        }


        // 6.10 Formats valid Service Cost input to exactly two decimal places.
        private void ServiceCost_LostFocus(
            object sender,
            RoutedEventArgs e
        )
        {
            string serviceCostText = txtServiceCost.Text.Trim();

            if (string.IsNullOrWhiteSpace(serviceCostText))
            {
                return;
            }

            bool validCost = double.TryParse(
                serviceCostText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out double serviceCost
            );

            if (validCost && serviceCost >= 0)
            {
                txtServiceCost.Text =
                    serviceCost.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        // 6.17 Clears all input textboxes after a service item has been added.
        private void ClearTextboxes()
        {
            txtClientName.Clear();
            txtDroneModel.Clear();
            txtServiceProblem.Clear();
            txtServiceCost.Clear();
            radRegular.IsChecked = true;
            txtClientName.Focus();
        }
    }
}