﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Network.Models;

namespace Microsoft.Azure.Commands.Network
{
    [Cmdlet(VerbsCommon.Set, "AzureRmNetworkInterfaceIpConfig"), OutputType(typeof(PSNetworkInterface))]
    public class SetAzureNetworkInterfaceIpConfigCommand : AzureNetworkInterfaceIpConfigBase
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the IpConfiguration")]
        [ValidateNotNullOrEmpty]
        public override string Name { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The Network Interface")]
        public PSNetworkInterface NetworkInterface { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            var ipconfig = this.NetworkInterface.IpConfigurations.SingleOrDefault(resource => string.Equals(resource.Name, this.Name, System.StringComparison.CurrentCultureIgnoreCase));

            if (ipconfig == null)
            {
                throw new ArgumentException("IpConfiguration with the specified name does not exist");
            }


            // Get the subnetId and publicIpAddressId from the object if specified
            if (string.Equals(ParameterSetName, "id"))
            {
                this.SubnetId = this.Subnet.Id;

                if (PublicIpAddress != null)
                {
                    this.PublicIpAddressId = this.PublicIpAddress.Id;
                }
            }

            ipconfig.Name = this.Name;

            if (!string.IsNullOrEmpty(this.SubnetId))
            {
                ipconfig.Subnet = new PSSubnet();
                ipconfig.Subnet.Id = this.SubnetId;

                if (!string.IsNullOrEmpty(this.PrivateIpAddress))
                {
                    ipconfig.PrivateIpAddress = this.PrivateIpAddress;
                    ipconfig.PrivateIpAllocationMethod = Management.Network.Models.IPAllocationMethod.Static;
                }
                else
                {
                    ipconfig.PrivateIpAllocationMethod = Management.Network.Models.IPAllocationMethod.Dynamic;
                }
            }

            if (!string.IsNullOrEmpty(this.PrivateIpAddress))
            {
                ipconfig.PrivateIpAddress = this.PrivateIpAddress;
            }

            ipconfig.Subnet = null;
            if (!string.IsNullOrEmpty(this.SubnetId))
            {
                ipconfig.Subnet = new PSSubnet();
                ipconfig.Subnet.Id = this.SubnetId;
            }

            ipconfig.PublicIpAddress = null;
            if (!string.IsNullOrEmpty(this.PublicIpAddressId))
            {
                ipconfig.PublicIpAddress = new PSPublicIpAddress();
                ipconfig.PublicIpAddress.Id = this.PublicIpAddressId;
            }

            ipconfig.PrivateIpAddressVersion = this.PrivateIpAddressVersion;

            WriteObject(this.NetworkInterface);
        }
    }
}
