import React, {useState, useEffect} from 'react';
import { useNavigate} from "react-router-dom";
import {Base_storage, User} from '../Variables';

function Purchases() {
  const [purchases, setPurchases] = useState([]);
  const navigate = useNavigate();

  useEffect(() =>{
    if(User.Uid === null || User.Uid === "")
    { 
        User.Uid = sessionStorage.getItem("Uid");
        User.Details = JSON.parse(sessionStorage.getItem("Details"));
        if(User.Uid === null || User.Uid === ""){
            console.log("User data could not be loaded from cache, redirecting!"); 
            navigate("/login");
        }
        else {console.log("User data loaded from cache!");}
    }

    fetch(Base_storage + "listsalepurchase?uid=" + User.Uid + "&type=purchase", {
        method:"GET"
    })
    .then(res => res.json())
    .then((response) =>{
        console.log(response.Message);
        setPurchases(response.SalesPurchases);
    })
    .catch((response) => {
        console.log(response.Message);
    });

}, [navigate]);

function SearchSubmit(e){
    e.preventDefault();
    e = e.target.elements;
    fetch(Base_storage + "listsalepurchase?uid=" + User.Uid + "&type=purchase&product=" + e.product.value + "&quantityover=" + e.qover.value
    + "&quantityunder=" + e.qunder.value + "&priceover=" + e.pover.value + "&priceunder=" + e.punder.value + "&before=" + e.before.value + "&after=" + e.after.value 
    + "&location=" + e.location.value + "&username=" + e.username.value + "&limit=" + e.limit.value
    ,{
        method:"GET"
    })
    .then(res => res.json())
    .then((response) =>{
        console.log(response.Message);
        setPurchases(response.SalesPurchases);
    })
    .catch((response) => {
        console.log(response.Message);
    });
}

return (
  <div>
      <div className='res-primary'>
          <ul className="list-group w-100">
              <div className="row border-bottom py-2 w-100">
                  <h5 className="col-xs-12 col-sm-1">Id</h5>
                  <h5 className="col-xs-12 col-sm-1">Product</h5>
                  <h5 className="col-xs-12 col-sm-2">Quantity</h5>
                  <h5 className="col-xs-12 col-sm-2">Total Price</h5>
                  <h5 className="col-xs-12 col-sm-2">Date</h5>
                  <h5 className="col-xs-12 col-sm-1">Location</h5>
                  <h5 className="col-xs-12 col-sm-2">User</h5>
              </div>
          </ul>
          <form onSubmit={SearchSubmit} id="filterForm">
              <ul className="list-group w-100">
                  <div className="row border-bottom py-2 w-100">
                      <div className='col-xs-12 col-sm-1'>
                          <button type="submit" className='btn btn-dark'>Filter</button>
                          <button type="Reset" className='btn btn-dark'>Reset</button>
                      </div>
                      <div className="col-xs-12 col-sm-1">
                          <label className='form-label mx-2' htmlFor="product">Name:</label>
                          <input type="text" name="product" className='w-50'/>
                      </div>
                      <div className="col-xs-12 col-sm-2">
                          <div className=' float-start'>
                              <label className='form-label me-1' htmlFor="qover">Over:</label>
                              <input type="number" name="qover" className='form-text w-25'/>
                              <label className='form-label ms-2 me-1' htmlFor="qunder">Under:</label>
                              <input type="number" name="qunder" className='form-text w-25'/>
                          </div>
                      </div>
                      <div className="col-xs-12 col-sm-2">
                          <div className=' float-start'>
                              <label className='form-label me-1' htmlFor="pover">Over:</label>
                              <input type="number" name="pover" className='form-text w-25'/>
                              <label className='form-label ms-2 me-1' htmlFor="punder">Under:</label>
                              <input type="number" name="punder" className='form-text w-25'/>
                          </div>
                      </div>
                      <div className="col-xs-12 col-sm-2">
                          <div className=' float-start'>
                              <label className='form-label me-1' htmlFor="before">Before:</label>
                              <input type="date" name="before" className='form-text'/><br/>
                              <label className='form-label ms-2 me-1' htmlFor="after">After:</label>
                              <input type="date" name="after" className='form-text'/>
                          </div>
                      </div>
                      <div className="col-xs-12 col-sm-1">
                          <label className='form-label mx-2' htmlFor="location">Name:</label>
                          <input type="text" name="location" className='w-50'/>
                      </div>
                      <div className="col-xs-12 col-sm-2">
                          <label className='form-label mx-2' htmlFor="username">Userame:</label>
                          <input type="text" name="username" className='w-50'/>
                      </div> 
                      <div className='col-xs-12 col-sm-1'>
                          <label className='form-label mx-2' htmlFor="limit">Limit:</label>
                          <input type="number" name="limit" className='form-text w-50'></input>
                      </div>
                  </div>
              </ul>
          </form>
		  <div className="list">
              {purchases.map((purchase) => (
                <div key={purchase.Id} className="row border-bottom  py-2 w-100">
                  <div className="col-xs-12 col-sm-1">{purchase.Id}</div>
                  <div className="col-xs-4 col-sm-1">{purchase.Product}</div>
                  <div className="col-xs-8 col-sm-2">{purchase.Quantity}</div>
                  <div className="col-xs-10 col-sm-2">{purchase.TotalPrice}</div>
                  <div className="col-xs-10 col-sm-2">{new Date(parseInt(purchase.Date.substring(6,19))).toDateString()}</div>
                  <div className="col-xs-10 col-sm-1">{purchase.Location}</div>
                  <div className="col-xs-10 col-sm-2">{purchase.Username}</div>
                </div>
              ))}
          </div>
      </div>
  </div>
  )
}

export default Purchases