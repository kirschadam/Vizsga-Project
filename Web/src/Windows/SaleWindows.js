import { Base_storage, User } from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_storage + "AddSalePurchase",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                type: "sale",
                product:e.product.value,
                quantity:e.quantity.value,
                location:e.location.value
            })
        })
        .then(res => res.json())
        .then((res) =>{
            alert(res);
            window.location.reload();
        })
        .catch((res) => {
            console.log(res);
        });
    }

    return(
		<BaseModal Submitted={Create} onClosed={onClose} title="Create Sale">

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
        e = e.target.elements;
		fetch(Base_storage + "UpdateSalePurchase",
		{
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: sale.Id,
				type: "sale",
				product: e.product.value,
				quantity: e.quantity.value,
				totalPrice: e.totalprice.value,
                date:e.date.value,
                location:e.location.value,
                username:e.username.value
		    })
		})
		.then(res => res.json())
		.then((res) =>{
			alert(res);
			window.location.reload();
		})
		.catch((res) => {
			console.log(res);
		});
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
        .then((res) =>{
            alert(res);
            window.location.reload();
        })
        .catch((res) => {
            console.log(res);
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose} title="Delete Sale">
            Are you sure to delete the sale of <b>{sale == null ? "" : sale.Quantity + " " + sale.Product}</b><br/>
            at location <b>{sale == null ? "" : sale.Location}</b>?
        </BaseModal>
    )
}