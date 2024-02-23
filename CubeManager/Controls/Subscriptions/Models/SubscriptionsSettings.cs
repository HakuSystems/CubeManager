using System.Drawing;

namespace CubeManager.Controls.Subscriptions.Models;

public class SubscriptionsSettings
{

    public string ActiveTransitionContent { get; set; }
    public Color LastBackgroundColor { get; set; }
    
    public double MonthlyCost { get; set; }
    public double YearlyCost { get; set; }
    public double TotalCost { get; set; }
    
    public int SubscriptionsCount { get; set; }
    public int SubscriptionsCountWithOneTimePayment { get; set; }
    public int SubscriptionsCountWithRecurringPayment { get; set; }
    public int SubscriptionsCountWithNextPaymentDateInNextMonth { get; set; }
    
}