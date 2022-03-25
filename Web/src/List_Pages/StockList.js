import React, {useState, useEffect, Fragment} from 'react';
import { useNavigate} from "react-router-dom";
import {Base_storage, User} from '../Variables';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faPencilAlt, faTrash } from "@fortawesome/free-solid-svg-icons";
import {CreateWindow, UpdateWindow, DeleteWindow} from "../Windows/StockWindows"

function Stocks() {
    const [stocks, setStocks] = useState([]);
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
		sessionStorage.setItem("route", "stocks")

        fetch(Base_storage + "liststock?uid=" + User.Uid + "&limit=50", {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            setStocks(response.Stocks.sort(function(a, b){
                return b.Id - a.Id;
            }));
        })
        .catch((response) => {
            console.log(response.Message);
        });

    }, [navigate]);

    function SearchSubmit(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_storage + "liststock?uid=" + User.Uid + "&product=" + e.product.value + "&location=" + e.location.value + 
        "&quantityover=" + e.over.value + "&quantityunder=" + e.under.value + "&limit=" + e.limit.value
        ,{
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            setStocks(response.Stocks.sort(function(a, b){
                return b.Id - a.Id;
            }));
        })
        .catch((response) => {
            console.log(response.Message);
        });
    }


	const [deletedisplay, setDeleteDisplay] = useState(false);
	const [createdisplay, setCreateDisplay] = useState(false);
	const [updatedisplay, setUpdateDisplay] = useState(false);
	const [stock, setStock] = useState();

	function Update(e){
		var element;
		if(e.target.tagName === "svg"){
			element = e.target.parentNode;
		}
		else if(e.target.tagName === "path"){
			element = e.target.parentNode.parentNode;
		}
		else element = e.target;

		var currid = element.parentNode.parentNode.firstChild.innerText;
		var stock = stocks.find((stck) => stck.Id === parseInt(currid));
		if(stock != null){
			setStock(stock);
			setUpdateDisplay(true);
		}
	}

	function Delete(e){
		var element;
		if(e.target.tagName === "svg"){
			element = e.target.parentNode;
		}
		else if(e.target.tagName === "path"){
			element = e.target.parentNode.parentNode;
		}
		else element = e.target;

		var currid = element.parentNode.parentNode.firstChild.innerText;
		var stock = stocks.find((stck) => stck.Id === parseInt(currid));
		if(stock != null){
			setStock(stock);
			setDeleteDisplay(true);
		}
	}

    return (
    <Fragment>
        <div>
            <div className='res-primary res-background'>
                <ul className="list-group w-100">
                    <div className="row border-bottom py-2 w-100">
                        <h5 className="col-xs-12 col-sm-1">Id</h5>
                        <h5 className="col-xs-4 col-sm-3">Quantity</h5>
                        <h5 className="col-xs-8 col-sm-3">Product</h5>
                        <h5 className="col-xs-10 col-sm-3">Location</h5>
					    <div className="col-xs-10 col-sm-2 ">
					        <button className="btn btn-sm btn-outline-warning mr-2" 
                              onClick={() => {setCreateDisplay(true)}}>
                                {<FontAwesomeIcon icon={faPlus} />}
                            </button>  
				  	    </div>
                    </div>
                </ul>
                <form onSubmit={SearchSubmit} id="filterForm">
                    <ul className="list-group w-100">
                        <div className="row border-bottom py-2 w-100">
                            <div className='col-xs-12 col-sm-1'>
                                <button type="submit" className='btn btn-dark'>Filter</button>
                                <button type="Reset" className='btn btn-dark'>Reset</button>
                            </div>
                            <div className="col-xs-4 col-sm-3">
                                <div className=' float-start'>
                                    <label className='form-label me-1' htmlFor="over">Over:</label>
                                    <input type="number" name="over" className='form-text w-25'/>
                                    <label className='form-label ms-2 me-1' htmlFor="under">Under:</label>
                                    <input type="number" name="under" className='form-text w-25'/>
                                </div>
                            </div>
                            <div className="col-xs-8 col-sm-3">
                                <label className='form-label mx-2' htmlFor="product">Name:</label>
                                <input type="text" name="product"/>
                            </div>
                            <div className="col-xs-12 col-sm-3">
                                <label className='form-label mx-2' htmlFor="location">Name:</label>
                                <input type="text" name="location"/>
                            </div> 
                            <div className='col-xs-12 col-sm-1'>
                                <label className='form-label mx-2' htmlFor="limit">Limit:</label>
                                <input type="number" name="limit" className='form-text w-50'></input>
                            </div>
                        </div>
                    </ul>
                </form>
			    <div className="list res-background">
                    {stocks.map((stock) => (
                        <div key={stock.Id} className="row border-bottom  py-2 w-100">
                            <div className="col-xs-12 col-sm-1">{stock.Id}</div>
                            <div className="col-xs-4 col-sm-3">{stock.Quantity}</div>
                            <div className="col-xs-8 col-sm-3">{stock.Product}</div>
                            <div className="col-xs-10 col-sm-3">{stock.Location}</div>
						    <div className="col-xs-10 col-sm-2">
							    <button className="btn btn-sm btn-outline-warning mr-2" onClick={Update}>	
						  		    {<FontAwesomeIcon icon={faPencilAlt} />}
					  	  	    </button>  	
						  	    <button className="btn btn-sm btn-outline-warning mr-2" onClick={Delete} >
								    {<FontAwesomeIcon icon={faTrash} />}
							    </button>  
						    </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
        
		<div className={createdisplay ? "visible" : "invisible"}><CreateWindow onClose={() => {setCreateDisplay(false)}}/></div>
		<div className={updatedisplay ? "visible" : "invisible"}><UpdateWindow stock={stock} onClose={() => {setUpdateDisplay(false)}}/></div>
		<div className={deletedisplay ? "visible" : "invisible"}><DeleteWindow stock={stock} onClose={() => {setDeleteDisplay(false)}}/></div>
    </Fragment>
    )
}

export default Stocks
