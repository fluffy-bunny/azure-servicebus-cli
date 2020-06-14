using Microsoft.Azure.Management.Fluent;
using System;
using System.Threading.Tasks;

namespace Common
{
    public static class AzureFluentExtensions
    {
        public async static Task GetScaleSetsForResourceGroupAsync(this IAzure azure, string resourceGroup)
        {
            try
            {
                var virtualMachineScaleSetsList =
                    await azure
                    .VirtualMachineScaleSets
                    .ListByResourceGroupAsync(resourceGroup);
                foreach (var item in virtualMachineScaleSetsList)
                {
                    Utilities.Log($"VMSS: {item.Name}\n Id: {item.Id}\n Capacity: {item.Capacity}");
                }

            }
            catch (Exception ex)
            {
                Utilities.Log(ex);
            }
        }

        public async static Task<Microsoft.Azure.Management.Compute.Fluent.IVirtualMachineScaleSet>
            GetScaleSetAsync(this IAzure azure, string resourceGroup, string scaleSet)
        {
            try
            {
                var subscriptionId = azure.SubscriptionId;
                var vmScaleSetId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.Compute/virtualMachineScaleSets/{scaleSet}";

                var item =
                          await azure
                          .VirtualMachineScaleSets
                          .GetByIdAsync(vmScaleSetId);
                Utilities.Log($"VMSS: {item.Name}\n Id: {item.Id}\n Capacity: {item.Capacity}");
                return item;
            }
            catch (Exception ex)
            {
                Utilities.Log(ex);
            }
            return null;
        }
    }
}
