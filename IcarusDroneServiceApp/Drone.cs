using System.Globalization;

namespace IcarusDroneServiceWPF
{
    // 6.1 Drone class used to store each drone service item.
    // The class uses private attributes with public getter and setter methods.
    public class Drone
    {
        private string clientName = "";
        private string droneModel = "";
        private string serviceProblem = "";
        private double serviceCost;
        private int serviceTag;

        public string ClientName
        {
            get { return GetClientName(); }
        }

        public string DroneModel
        {
            get { return GetDroneModel(); }
        }

        public string ServiceProblem
        {
            get { return GetServiceProblem(); }
        }

        public double ServiceCost
        {
            get { return GetServiceCost(); }
        }

        public string ServiceCostDisplay
        {
            get { return serviceCost.ToString("0.00"); }
        }

        public int ServiceTag
        {
            get { return GetServiceTag(); }
        }

        public Drone()
        {
            clientName = "";
            droneModel = "";
            serviceProblem = "";
            serviceCost = 0.00;
            serviceTag = 100;
        }

        public Drone(string clientName, string droneModel, string serviceProblem, double serviceCost, int serviceTag)
        {
            SetClientName(clientName);
            SetDroneModel(droneModel);
            SetServiceProblem(serviceProblem);
            SetServiceCost(serviceCost);
            SetServiceTag(serviceTag);
        }

        public string GetClientName()
        {
            return clientName;
        }

        // 6.1 Formats the client name into Title Case.
        public void SetClientName(string value)
        {
            clientName = ToTitleCase(value);
        }

        public string GetDroneModel()
        {
            return droneModel;
        }

        public void SetDroneModel(string value)
        {
            droneModel = value.Trim();
        }

        public string GetServiceProblem()
        {
            return serviceProblem;
        }

        // 6.1 Formats the service problem into Sentence case.
        public void SetServiceProblem(string value)
        {
            serviceProblem = ToSentenceCase(value);
        }

        public double GetServiceCost()
        {
            return serviceCost;
        }

        public void SetServiceCost(double value)
        {
            serviceCost = value;
        }

        public int GetServiceTag()
        {
            return serviceTag;
        }

        public void SetServiceTag(int value)
        {
            serviceTag = value;
        }

        // 6.1 Returns the Client Name and Service Cost for the finished ListBox.
        public string Display()
        {
            return clientName + " - $" + serviceCost.ToString("0.00");
        }

        private string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(value.Trim().ToLower());
        }

        private string ToSentenceCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            value = value.Trim().ToLower();

            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}