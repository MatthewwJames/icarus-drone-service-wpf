using IcarusDroneServiceWPF;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace IcarusDroneServiceApp
{
    public partial class MainWindow : Window
    {
        // 6.2 Global List<T> of type Drone called FinishedList.
        private readonly List<Drone> FinishedList = new List<Drone>();

        // 6.3 Global Queue<T> of type Drone called RegularService.
        private readonly Queue<Drone> RegularService = new Queue<Drone>();

        // 6.4 Global Queue<T> of type Drone called ExpressService.
        private readonly Queue<Drone> ExpressService = new Queue<Drone>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // 6.5 Adds a new service item to the correct Queue based on selected priority.
        // 6.6 Adds 15% to the service cost before adding an Express service item.
        // 6.7 Calls GetServicePriority before adding the item to a Queue.
        // 6.11 Calls IncrementServiceTag before completing the add process.
        // 6.17 Calls ClearTextboxes after a service item has been added.
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

            if (!int.TryParse(txtServiceTag.Text, out int serviceTag))
            {
                txtStatus.Text = "Service tag is invalid.";
                return;
            }

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

        // 6.7 Returns the value of the selected priority radio button.
        private string GetServicePriority()
        {
            if (radExpress.IsChecked == true)
            {
                return "Express";
            }

            return "Regular";
        }

        // 6.10 Checks that the Service Cost textbox contains a valid double value.
        private bool ValidateServiceCost(out double serviceCost)
        {
            serviceCost = 0;

            if (string.IsNullOrWhiteSpace(txtServiceCost.Text))
            {
                txtStatus.Text = "Service cost is required.";
                return false;
            }

            if (!double.TryParse(txtServiceCost.Text, out serviceCost))
            {
                txtStatus.Text = "Service cost must be a number.";
                return false;
            }

            if (serviceCost < 0)
            {
                txtStatus.Text = "Service cost cannot be negative.";
                return false;
            }

            serviceCost = double.Parse(serviceCost.ToString("0.00"));
            return true;
        }

        // 6.11 Increments the service tag by 10 after a new service item is added.
        private void IncrementServiceTag()
        {
            int currentTag = int.Parse(txtServiceTag.Text);

            if (currentTag < 900)
            {
                currentTag += 10;
            }

            txtServiceTag.Text = currentTag.ToString();
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
        private void ServiceCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string newText = txtServiceCost.Text.Insert(txtServiceCost.SelectionStart, e.Text);

            Regex regex = new Regex(@"^\d*\.?\d{0,2}$");

            e.Handled = !regex.IsMatch(newText);
        }

        // 6.10 Formats the Service Cost textbox to two decimal places when the user leaves the textbox.
        private void ServiceCost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtServiceCost.Text, out double serviceCost))
            {
                txtServiceCost.Text = serviceCost.ToString("0.00");
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