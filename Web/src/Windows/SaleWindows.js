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
                type: "sale",
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
        })
        .catch((response) => {
            console.log(response);
            return;
        });
        
        window.location.reload();
    }

    return (
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create Sale">
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
		add sale?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, sale}){

    function Update(e){
        e.preventDefault();
		fetch(Base_storage + "UpdateSalePurchase",
		{
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: sale.Id,
				type: "sale",
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
		})
		.catch((response) => {
			console.log(response);
			return;
		});
		window.location.reload();
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Sale">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Product:</label>
                <div className="col-sm-9">
                    <input type="text" name="product" className="form-control" defaultValue={sale == null ? "" : sale.Product} />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Quantity:</label>
                <div className="col-sm-9">
                    <input type="number" name="quantity" className="form-control" defaultValue={sale == null ? "" : sale.Quantity} />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">TotalPrice:</label>
                <div className="col-sm-9">
                    <input type="number" name="totalprice" className="form-control" defaultValue={sale == null ? "" : sale.TotalPrice}/>
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
                    <input type="text" name="location" className="form-control" defaultValue={sale == null ? "" : sale.Location}/>
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Username:</label>
                <div className="col-sm-9">
                    <input type="text" name="username" className="form-control" defaultValue={sale == null ? "" : sale.Username}/>
                </div>
            </div>
		update sale?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, sale}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_storage + "DeleteSalePurchase",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                type: "sale",
                id: sale.Id
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
        window.location.reload();
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete Sale">
            Are you sure to delete the sale of <span><b>{sale == null ? "" : sale.Quantity + " " + sale.Product}</b></span>
            <br/>at location <span><b>{sale == null ? "" : sale.Location}</b></span>?
        </BaseModal>
    )
}