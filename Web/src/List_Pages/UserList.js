import { useState, useEffect, Fragment } from 'react';
import { useNavigate } from "react-router-dom";
import { Base_user, User } from '../Variables';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faPencilAlt, faTrash } from "@fortawesome/free-solid-svg-icons";
import { CreateWindow, UpdateWindow, DeleteWindow } from "../Windows/UserWindows"

function Users({currentpage}) {
  	const [users, setUsers] = useState([]);
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
		if(User.Details.Permission  < 9){
			alert("Permission level low, redirecting!");
			navigate("/products");
			return;
		}

		//Set the page we are on, used for the selection on the navbar
		currentpage("user");

		//Basic fetch of everything in table
    	fetch(Base_user + "listuser?uid=" + User.Uid ,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
			  
			//Set order of items to descending based on Id
      		setUsers(response.Users.sort(function(a, b){
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
    	fetch(Base_user + "listuser?uid=" + User.Uid + "&username=" + e.username.value + "&location=" + e.location.value +
			"&permissionover=" + e.over.value + "&permissionunder=" + e.under.value + "&active=" + e.active.value + "&limit=" + e.limit.value
    	,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
			  
			//Set order of items to descending based on Id
      		setUsers(response.Users.sort(function(a, b){
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
	const [user, setUser] = useState();
  
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
		var user = users.find((usr) => usr.Id === parseInt(currid));
		if(user != null){
			setUser(user);
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
		var user = users.find((usr) => usr.Id === parseInt(currid));
		if(user != null){
			setUser(user);
			setDeleteDisplay(true);
		}
	}

  	return (
		<Fragment>
    		<div className='res-primary res-background'>
        		<div className="row border-bottom py-2 w-100">

          			<h5 className="col-xs-12 col-sm-1">Id</h5>
          			<h5 className="col-xs-4 col-sm-3">Username</h5>
          			<h5 className="col-xs-8 col-sm-3">Location</h5>
          			<h5 className="col-xs-10 col-sm-3">Permission</h5>
          			<h5 className="col-xs-10 col-sm-1">Status</h5>
					<div className="col-xs-10 col-sm-1">
					  	<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 8 ? "" : "invisible")} 
						  onClick={() => {setCreateDisplay(true)}}>
						  	{<FontAwesomeIcon icon={faPlus} />}
						</button>  
				  	</div>

        		</div>


      			<form onSubmit={SearchSubmit} id="filterForm">
          			<div className="row border-bottom py-2 w-100">

            			<div className='col-xs-12 col-sm-1'>
              				<button type="submit" className='btn btn-dark'>Filter</button>
              				<button type="Reset" className='btn btn-dark'>Reset</button>
            			</div>
						
            			<div className="col-xs-8 col-sm-3">
              				<label className='form-label mx-2' htmlFor="username">Name:</label>
              				<input type="text" name="username"/>
            			</div>

            			<div className="col-xs-8 col-sm-3">
              				<label className='form-label mx-2' htmlFor="location">Name:</label>
              				<input type="text" name="location"/>
            			</div>

            			<div className="col-xs-4 col-sm-3">
              				<div className=' float-start'>
                				<label className='form-label me-1' htmlFor="over">Over:</label>
                				<input type="number" name="over" className='form-text w-25'/>
                				<label className='form-label ms-2 me-1' htmlFor="under">Under:</label>
                				<input type="number" name="under" className='form-text w-25'/>
              				</div>
            			</div>

            			<div className="col-xs-8 col-sm-1">
              				<label className='form-label mx-2' htmlFor="active">Active?(1/0)</label>
              				<input type="text" name="active" className='w-50'/>
            			</div>

            			<div className='col-xs-12 col-sm-1'>
              				<label className='form-label mx-2' htmlFor="limit">Limit:</label>
              				<input type="number" name="limit" className='form-text w-50'></input>
            			</div>

         	 		</div>
      			</form>


				<div className="list res-background">

					{users.map((user) => (
						<div key={user.Id} className="row border-bottom  py-2 w-100">
							<div className="col-xs-12 col-sm-1">{user.Id}</div>
							<div className="col-xs-4 col-sm-3">{user.Username}</div>
							<div className="col-xs-8 col-sm-3">{user.Location}</div>
							<div className="col-xs-10 col-sm-3">{user.Permission}</div>
							<div className="col-xs-10 col-sm-1">{user.Active === 1 ? "Active" : "Inactive"}</div>
							<div className="col-xs-10 col-sm-1">
								<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 8 ? "" : "invisible")} onClick={Update}>	
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
			<div className={updatedisplay ? "" : "invisible"}><UpdateWindow user={user} onClose={() => {setUpdateDisplay(false)}}/></div>
			<div className={deletedisplay ? "" : "invisible"}><DeleteWindow user={user} onClose={() => {setDeleteDisplay(false)}}/></div>
		</Fragment>
  	)
}

export default Users