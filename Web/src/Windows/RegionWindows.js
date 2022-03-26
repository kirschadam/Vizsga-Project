import { Base_location, User } from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_location + "AddRegion",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                region: e.region.value
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
		<BaseModal Submitted={Create} onClosed={onClose} title="Create Region">

			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" />
                </div>
            </div>

		    add region?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, region}){

    function Update(e){
        e.preventDefault();
        e = e.target.elements;
		fetch(Base_location + "UpdateRegion",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: region.Id,
				region: e.region.value
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
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Region">

			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" defaultValue={region == null ? "" : region.Name} />
                </div>
            </div>

		    update region?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, region}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_location + "DeleteRegion",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: region.Id
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
        <BaseModal Submitted={Delete} onClosed={onClose} title="Delete Region">
            Are you sure to delete the region <b>{region == null ? "" : region.Name}</b>?
        </BaseModal>
    )
}