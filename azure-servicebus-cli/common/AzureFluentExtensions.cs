using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public static class AzureFluentExtensions
    {
        public async static Task<List<IVirtualMachineScaleSet>> GetScaleSetsForResourceGroupAsync(this IAzure azure, string resourceGroup)
        {
            var result = new List<IVirtualMachineScaleSet>();

            var rg = await azure.ResourceGroups.GetByNameAsync(resourceGroup);
            if (rg == null)
            {
                throw new Exception($"rg:{resourceGroup} does not exist!");
            }


            var virtualMachineScaleSetsList =
                await azure.VirtualMachineScaleSets
                .ListByResourceGroupAsync(rg.Name);
            if (virtualMachineScaleSetsList != null)
            {
                foreach (var item in virtualMachineScaleSetsList)
                {
                    result.Add(item);
                }
                do
                {
                    virtualMachineScaleSetsList = await virtualMachineScaleSetsList.GetNextPageAsync();
                    if (virtualMachineScaleSetsList == null)
                    {
                        break;
                    }
                    foreach (var item in virtualMachineScaleSetsList)
                    {
                        result.Add(item);
                    }
                } while (true);
            }

            return result;
        }

        public async static Task<List<IVirtualMachineScaleSetVM>> GetVirtualMachineScaleSetVMs(this IVirtualMachineScaleSet vmss)
        {
            var result = new List<IVirtualMachineScaleSetVM>();
            var page = await vmss.VirtualMachines.ListAsync();
            if(page != null)
            {
                do
                {
                    foreach(var item in page)
                    {
                        result.Add(item);
                    }
                    page = await page.GetNextPageAsync();
                } while (page != null);
            }
            return result;
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
