import React, {useState, useEffect} from 'react';
import { useNavigate, NavLink} from "react-router-dom";
import {Base_storage, User} from './Variables';
import {Logout} from './Login_Logout';

function Test() {
    const [stocks, setStocks] = useState([]);
    const [isPending, setPending] = useState(false);
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

        setPending(true);
        fetch(Base_storage + "liststock?uid=" + User.Uid, {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            setStocks(response.Stocks);
        })
        .catch((response) => {
            console.log(response.Message);
        });
        setPending(false);

    }, [navigate]);

    function SearchSubmit(e){
        e.preventDefault();
        setPending(true);
        fetch(Base_storage + "liststock?uid=" + User.Uid + "&product=" + e.target.elements.product.value + "&location=" + e.target.elements.location.value + 
        "&quantityover=" + e.target.elements.over.value + "&quantityunder=" + e.target.elements.under.value + "&limit=" + e.target.elements.limit.value, {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            setStocks(response.Stocks);
        })
        .catch((response) => {
            console.log(response.Message);
            navigate("/test");
        });
        setPending(false);
    }
    
    if(isPending || !stocks.length){
        return(
            <div className="center-item">
                <div className="spinner-border text-danger"></div>
            </div>
        );
    }

    return (
    <div>
        <div className="col-1">
            <Logout/>
        </div>
        <div className='res-primary'>
            <ul className="list-group w-100">
                <div className="row border-bottom py-2 w-100">
                    <h5 className="col-xs-12 col-sm-1">Id</h5>
                    <h5 className="col-xs-4 col-sm-2">Quantity</h5>
                    <h5 className="col-xs-8 col-sm-4">Product</h5>
                    <h5 className="col-xs-10 col-sm-4">Location</h5>
                </div>
            </ul>
            <form onSubmit={SearchSubmit}>
            <ul className="list-group w-100">
                <div className="row border-bottom py-2 w-100">
                    <div className='col-xs-12 col-sm-1'>
                        <button type="submit" className='btn btn-dark'>Filter</button>
                    </div>
                    <div className="col-xs-4 col-sm-2">
                        <div>
                            <label className='form-label  me-1'>Under:</label>
                            <input type="text" name="under" className='form-text w-25'/>
                            <label className='form-label ms-2 me-1'>Over:</label>
                            <input type="text" name="over" className='form-text w-25'/>
                        </div>
                    </div>
                    <div className="col-xs-8 col-sm-4">
                        <label className='form-label mx-2'>Name:</label>
                        <input type="text" name="product"/>
                    </div>
                    <div className="col-xs-12 col-sm-4">
                        <label className='form-label mx-2'>Name:</label>
                        <input type="text" name="location"/>
                    </div> 
                    <div className='col-xs-12 col-sm-1'>
                        <label className='form-label mx-2'>Limit:</label>
                        <input type="text" name="limit" className='form-text w-50'></input>
                    </div>
                </div>
            </ul>
            </form>
            {stocks.map((stock) => (
                <NavLink key={stock.Id} to={"/stock-" + stock.Id} className="text-decoration-none res-primary res-color-hover">
                    <div className="row border-bottom  py-2 w-100">
                        <div className="col-xs-12 col-sm-1">{stock.Id}</div>
                        <div className="col-xs-4 col-sm-2">{stock.Quantity}</div>
                        <div className="col-xs-8 col-sm-4">{stock.Product}</div>
                        <div className="col-xs-10 col-sm-4">{stock.Location}</div>
                    </div>
                </NavLink>
            ))}
        </div>
    </div>
    )
}

export default Test
