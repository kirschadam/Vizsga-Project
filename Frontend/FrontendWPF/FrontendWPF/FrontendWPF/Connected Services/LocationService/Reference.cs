﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrontendWPF.LocationService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Response_Location", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Response_Location : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.LocationService.Store[] LocationsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.LocationService.Store[] Locations {
            get {
                return this.LocationsField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationsField, value) != true)) {
                    this.LocationsField = value;
                    this.RaisePropertyChanged("Locations");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Store", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Store : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RegionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Region {
            get {
                return this.RegionField;
            }
            set {
                if ((object.ReferenceEquals(this.RegionField, value) != true)) {
                    this.RegionField = value;
                    this.RaisePropertyChanged("Region");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Response_Region", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Response_Region : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.LocationService.Region[] RegionsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.LocationService.Region[] Regions {
            get {
                return this.RegionsField;
            }
            set {
                if ((object.ReferenceEquals(this.RegionsField, value) != true)) {
                    this.RegionsField = value;
                    this.RaisePropertyChanged("Regions");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Region", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Region : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.LocationService.Store[] LocationsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.LocationService.Store[] Locations {
            get {
                return this.LocationsField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationsField, value) != true)) {
                    this.LocationsField = value;
                    this.RaisePropertyChanged("Locations");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LocationService.ILocationService")]
    public interface ILocationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/AddLocation", ReplyAction="http://tempuri.org/ILocationService/AddLocationResponse")]
        string AddLocation(string uid, string location, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/AddLocation", ReplyAction="http://tempuri.org/ILocationService/AddLocationResponse")]
        System.Threading.Tasks.Task<string> AddLocationAsync(string uid, string location, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/AddRegion", ReplyAction="http://tempuri.org/ILocationService/AddRegionResponse")]
        string AddRegion(string uid, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/AddRegion", ReplyAction="http://tempuri.org/ILocationService/AddRegionResponse")]
        System.Threading.Tasks.Task<string> AddRegionAsync(string uid, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/ListLocation", ReplyAction="http://tempuri.org/ILocationService/ListLocationResponse")]
        FrontendWPF.LocationService.Response_Location ListLocation(string uid, string id, string location, string region, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/ListLocation", ReplyAction="http://tempuri.org/ILocationService/ListLocationResponse")]
        System.Threading.Tasks.Task<FrontendWPF.LocationService.Response_Location> ListLocationAsync(string uid, string id, string location, string region, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/ListRegion", ReplyAction="http://tempuri.org/ILocationService/ListRegionResponse")]
        FrontendWPF.LocationService.Response_Region ListRegion(string uid, string id, string region, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/ListRegion", ReplyAction="http://tempuri.org/ILocationService/ListRegionResponse")]
        System.Threading.Tasks.Task<FrontendWPF.LocationService.Response_Region> ListRegionAsync(string uid, string id, string region, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/RemoveLocation", ReplyAction="http://tempuri.org/ILocationService/RemoveLocationResponse")]
        string RemoveLocation(string uid, string id, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/RemoveLocation", ReplyAction="http://tempuri.org/ILocationService/RemoveLocationResponse")]
        System.Threading.Tasks.Task<string> RemoveLocationAsync(string uid, string id, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/RemoveRegion", ReplyAction="http://tempuri.org/ILocationService/RemoveRegionResponse")]
        string RemoveRegion(string uid, string id, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/RemoveRegion", ReplyAction="http://tempuri.org/ILocationService/RemoveRegionResponse")]
        System.Threading.Tasks.Task<string> RemoveRegionAsync(string uid, string id, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/UpdateLocation", ReplyAction="http://tempuri.org/ILocationService/UpdateLocationResponse")]
        string UpdateLocation(string uid, string id, string location, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/UpdateLocation", ReplyAction="http://tempuri.org/ILocationService/UpdateLocationResponse")]
        System.Threading.Tasks.Task<string> UpdateLocationAsync(string uid, string id, string location, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/UpdateRegion", ReplyAction="http://tempuri.org/ILocationService/UpdateRegionResponse")]
        string UpdateRegion(string uid, string id, string region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILocationService/UpdateRegion", ReplyAction="http://tempuri.org/ILocationService/UpdateRegionResponse")]
        System.Threading.Tasks.Task<string> UpdateRegionAsync(string uid, string id, string region);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILocationServiceChannel : FrontendWPF.LocationService.ILocationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LocationServiceClient : System.ServiceModel.ClientBase<FrontendWPF.LocationService.ILocationService>, FrontendWPF.LocationService.ILocationService {
        
        public LocationServiceClient() {
        }
        
        public LocationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LocationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AddLocation(string uid, string location, string region) {
            return base.Channel.AddLocation(uid, location, region);
        }
        
        public System.Threading.Tasks.Task<string> AddLocationAsync(string uid, string location, string region) {
            return base.Channel.AddLocationAsync(uid, location, region);
        }
        
        public string AddRegion(string uid, string region) {
            return base.Channel.AddRegion(uid, region);
        }
        
        public System.Threading.Tasks.Task<string> AddRegionAsync(string uid, string region) {
            return base.Channel.AddRegionAsync(uid, region);
        }
        
        public FrontendWPF.LocationService.Response_Location ListLocation(string uid, string id, string location, string region, string limit) {
            return base.Channel.ListLocation(uid, id, location, region, limit);
        }
        
        public System.Threading.Tasks.Task<FrontendWPF.LocationService.Response_Location> ListLocationAsync(string uid, string id, string location, string region, string limit) {
            return base.Channel.ListLocationAsync(uid, id, location, region, limit);
        }
        
        public FrontendWPF.LocationService.Response_Region ListRegion(string uid, string id, string region, string limit) {
            return base.Channel.ListRegion(uid, id, region, limit);
        }
        
        public System.Threading.Tasks.Task<FrontendWPF.LocationService.Response_Region> ListRegionAsync(string uid, string id, string region, string limit) {
            return base.Channel.ListRegionAsync(uid, id, region, limit);
        }
        
        public string RemoveLocation(string uid, string id, string location) {
            return base.Channel.RemoveLocation(uid, id, location);
        }
        
        public System.Threading.Tasks.Task<string> RemoveLocationAsync(string uid, string id, string location) {
            return base.Channel.RemoveLocationAsync(uid, id, location);
        }
        
        public string RemoveRegion(string uid, string id, string region) {
            return base.Channel.RemoveRegion(uid, id, region);
        }
        
        public System.Threading.Tasks.Task<string> RemoveRegionAsync(string uid, string id, string region) {
            return base.Channel.RemoveRegionAsync(uid, id, region);
        }
        
        public string UpdateLocation(string uid, string id, string location, string region) {
            return base.Channel.UpdateLocation(uid, id, location, region);
        }
        
        public System.Threading.Tasks.Task<string> UpdateLocationAsync(string uid, string id, string location, string region) {
            return base.Channel.UpdateLocationAsync(uid, id, location, region);
        }
        
        public string UpdateRegion(string uid, string id, string region) {
            return base.Channel.UpdateRegion(uid, id, region);
        }
        
        public System.Threading.Tasks.Task<string> UpdateRegionAsync(string uid, string id, string region) {
            return base.Channel.UpdateRegionAsync(uid, id, region);
        }
    }
}
