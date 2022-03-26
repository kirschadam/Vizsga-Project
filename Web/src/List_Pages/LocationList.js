import { useState, useEffect, Fragment } from 'react';
import { useNavigate} from "react-router-dom";
import { Base_location, User } from '../Variables';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faPencilAlt, faTrash } from "@fortawesome/free-solid-svg-icons";
import { CreateWindow, UpdateWindow, DeleteWindow } from "../Windows/LocationWindows"

function Locations({currentpage}) {
	const [locations, setLocations] = useState([]);
	const navigate = useNavigate();

	useEffect(() =>{
		//Check if session storage has user data and load it, if not, return user to login screen
		if(User.Uid === null || User.Uid === "")
		{ 
			User.Uid = sessionStorage.getItem("Uid");
			User.Details = JSON.parse(sessionStorage.getItem("Details"));
			if(User.Uid === null || User.Uid === ""){
				console.log("User data could not be loaded from cache, redirecting!"); 
				navigate("/login");
      		}
      		else{console.log("User data loaded from cache!");}
    	}

		//Check if user is permitted to be on this page
		if(User.Details.Permission < 9){
			alert("Permission level low, redirecting!");
			navigate("/products");
			return;
		}

		//Set the page we are on, used for the selection on the navbar
		currentpage("location");

		//Basic fetch of everything in table
    	fetch(Base_location + "listlocation?uid=" + User.Uid ,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);

			//Set order of items to descending based on Id
      		setLocations(response.Locations.sort(function(a, b){
                return b.Id - a.Id;
            }).slice(0, 50));
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}, [navigate, currentpage]);

	//A secondary, more complicated fetch using the form inputs above the list of items
  	function SearchSubmit(e){
    	e.preventDefault();
    	e = e.target.elements;
    	fetch(Base_location + "listlocation?uid=" + User.Uid + "&location=" + e.location.value + "&region=" + e.region.value + "&limit=" + e.limit.value
    	,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
			  
			//Set order of items to descending based on Id
      		setLocations(response.Locations.sort(function(a, b){
                return b.Id - a.Id;
            }));
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}


	//Constants used for the update/add/remove pop up windows
	const [deletedisplay, setDeleteDisplay] = useState(false);
	const [createdisplay, setCreateDisplay] = useState(false);
	const [updatedisplay, setUpdateDisplay] = useState(false);
	const [location, setLocation] = useState();
  
	//Start process of updating the item where we clicked on the button
	function Update(e){
		var element;
		//check what part of the button we pressed
		//as depending on the part, we have different amounts of parent nodes to get through
		if(e.target.tagName === "svg"){
			element = e.target.parentNode;
		}
		else if(e.target.tagName === "path"){
			element = e.target.parentNode.parentNode;
		}
		else element = e.target;
  
		//get to the div that is the parent of an entire list item, and get it's first child, which contains the id of the item
		//Search for it in the array and set the constants accordingly
		var currid = element.parentNode.parentNode.firstChild.innerText;
		var location = locations.find((loc) => loc.Id === parseInt(currid));
		if(location != null){
			setLocation(location);
			setUpdateDisplay(true);
		}
	}
  
	//Start process of deleting the item where we clicked on the button
	function Delete(e){
		var element;
		//check what part of the button we pressed
		//as depending on the part, we have different amounts of parent nodes to get through
		if(e.target.tagName === "svg"){
			element = e.target.parentNode;
		}
		else if(e.target.tagName === "path"){
			element = e.target.parentNode.parentNode;
		}
		else element = e.target;
  
		//get to the div that is the parent of an entire list item, and get it's first child, which contains the id of the item
		//Search for it in the array and set the constants accordingly
		var currid = element.parentNode.parentNode.firstChild.innerText;
		var location = locations.find((loc) => loc.Id === parseInt(currid));
		if(location != null){
			setLocation(location);
			setDeleteDisplay(true);
		}
	}

  	return (
		<Fragment>
			<div className='res-primary res-background'>
				<div className="row border-bottom py-2 w-100">

					<h5 className="col-xs-12 col-sm-2">Id</h5>
					<h5 className="col-xs-4 col-sm-4">Location</h5>
					<h5 className="col-xs-8 col-sm-4">Region</h5>
					<div className="col-xs-10 col-sm-2 ">
						<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 8 ? "" : "invisible")} 
						onClick={() => {setCreateDisplay(true)}}>
							{<FontAwesomeIcon icon={faPlus} />}
						</button>  
					</div>
					
				</div>


				<form onSubmit={SearchSubmit} id="filterForm">
					<div className="row border-bottom py-2 w-100">

						<div className='col-xs-12 col-sm-2'>
							<button type="submit" className='btn btn-dark'>Filter</button>
							<button type="Reset" className='btn btn-dark'>Reset</button>
						</div>

						<div className="col-xs-8 col-sm-4">
							<label className='form-label mx-2' htmlFor="location">Name:</label>
							<input type="text" name="location"/>
						</div>

						<div className="col-xs-8 col-sm-4">
							<label className='form-label mx-2' htmlFor="region">Name:</label>
							<input type="text" name="region"/>
						</div>

						<div className='col-xs-12 col-sm-2'>
							<label className='form-label mx-2' htmlFor="limit">Limit:</label>
							<input type="number" name="limit" className='form-text w-50'></input>
						</div>

					</div>
				</form>


				<div className="list res-background">

					{locations.map((location) => (
						<div key={location.Id} className="row border-bottom  py-2 w-100">
							<div className="col-xs-12 col-sm-2">{location.Id}</div>
							<div className="col-xs-4 col-sm-4">{location.Name}</div>
							<div className="col-xs-8 col-sm-4">{location.Region}</div>
							<div className="col-xs-10 col-sm-2">
								<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 2 ? "" : "invisible")} onClick={Update}>	
									{<FontAwesomeIcon icon={faPencilAlt} />}
								</button>  	
								<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 8 ? "" : "invisible")} onClick={Delete} >
									{<FontAwesomeIcon icon={faTrash} />}
								</button>  
							</div>
						</div>
					))}

				</div>
			</div>
			
			<div className={createdisplay ? "" : "invisible"}><CreateWindow onClose={() => {setCreateDisplay(false)}}/></div>
			<div className={updatedisplay ? "" : "invisible"}><UpdateWindow location={location} onClose={() => {setUpdateDisplay(false)}}/></div>
			<div className={deletedisplay ? "" : "invisible"}><DeleteWindow location={location} onClose={() => {setDeleteDisplay(false)}}/></div>
		</Fragment>
  	)
}

export default Locations