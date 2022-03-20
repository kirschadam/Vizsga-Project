import React from 'react';
import {Base_storage, User} from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e)
    {
        e.preventDefault();
        fetch(Base_storage + "AddSalePurchase",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                type: "purchase",
                product:e.target.elements.product.value,
                quantity:e.target.elements.quantity.value,
                location:e.target.elements.location.value
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                window.location.reload();
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return (
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create Purchase">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Product:</label>
                <div className="col-sm-9">
                    <input type="text" name="product" className="form-control" />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Quantity:</label>
                <div className="col-sm-9">
                    <input type="number" name="quantity" className="form-control" min="0" max="10000000" />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" min="0" max="10000000" />
                </div>
            </div>
		add purchase?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, purchase}){

    function Update(e){
        e.preventDefault();
		fetch(Base_storage + "UpdateSalePurchase",
		{
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: purchase.Id,
				type: "purchase",
				product: e.target.elements.product.value,
				quantity: e.target.elements.quantity.value,
				totalPrice: e.target.elements.totalprice.value,
                date:e.target.elements.date.value,
                location:e.target.elements.location.value,
                username:e.target.elements.username.value
		    })
		})
		.then(res => res.json())
		.then((response) =>{
			alert(response);
			if(response === "X"){
				alert("This is a specifice alert!");
				return;
			}
            else{
                window.location.reload();
            }
		})
		.catch((response) => {
			console.log(response);
			return;
		});
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Purchase">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Product:</label>
                <div className="col-sm-9">
                    <input type="text" name="product" className="form-control" defaultValue={purchase == null ? "" : purchase.Product} />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Quantity:</label>
                <div className="col-sm-9">
                    <input type="number" name="quantity" className="form-control" defaultValue={purchase == null ? "" : purchase.Quantity} />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">TotalPrice:</label>
                <div className="col-sm-9">
                    <input type="number" name="totalprice" className="form-control" defaultValue={purchase == null ? "" : purchase.TotalPrice}/>
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Date:</label>
                <div className="col-sm-9">
                    <input type="date" name="date" className="form-control" defaultValue=""/>
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" defaultValue={purchase == null ? "" : purchase.Location}/>
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Username:</label>
                <div className="col-sm-9">
                    <input type="text" name="username" className="form-control" defaultValue={purchase == null ? "" : purchase.Username}/>
                </div>
            </div>
		update purchase?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, purchase}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_storage + "DeleteSalePurchase",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                type: "purchase",
                id: purchase.Id
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                window.location.reload();
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete Purchase">
            Are you sure to delete the purchase of <span><b>{purchase == null ? "" : purchase.Quantity + " " + purchase.Product}</b></span>
            <br/>at location <span><b>{purchase == null ? "" : purchase.Location}</b></span>?
        </BaseModal>
    )
}