import { useEffect } from 'react';
import { useNavigate} from "react-router-dom";
import CryptoJS from 'react-native-crypto-js';
import { Slide } from 'react-slideshow-image';
import 'react-slideshow-image/dist/styles.css';
import {User, Base_user, SlideImages} from './Variables';

export function Login( { headtext } ) {
    const navigate = useNavigate();

    //On load, if there is a stored Uid already, move to the products page and make navbar and logout button visible
    useEffect(() =>{
        if(sessionStorage.getItem("Uid") != null){

            document.getElementById("logout").style.visibility="visible";
            document.getElementById("navbar").style.visibility="visible";

            navigate("/products");
        }
        else{

            document.getElementById("logout").style.visibility="hidden";
            document.getElementById("navbar").style.visibility="hidden";
        }
    }, [navigate])


    function loginFormSubmit(e) {
        e.preventDefault();

        //Submit the username and the password as an MD5 hash
        fetch(Base_user + "loginuser", {
            method:"POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                username: e.target.elements.username.value,
                password: String(CryptoJS.MD5(e.target.elements.password.value))
            })
        })
        //Convert response and if we got back a user, and that user is active, store it in session storage and global variable
        //Also set the headtext and navigate to the products tab
        .then(res => res.json())
        .then((response) =>{
            if(response.User.Active === 0){
                alert("This user is currently set to inactive!");
                return;
            }
            else alert(response.Message);

            User.Uid = response.Uid;
            User.Details = response.User;

            //On successful login, save user data to session storage and make navbar and logout button visible, also move to products tab
            if(User.Uid !== null && User.Uid !== ""){
                sessionStorage.setItem("Uid", response.Uid);
                sessionStorage.setItem("Details", JSON.stringify(response.User));

                document.getElementById("logout").style.visibility="visible";
                document.getElementById("navbar").style.visibility="visible";

                headtext("Logged in user: " + User.Details.Username + " (" + User.Details.Permission + ")")
                navigate("/products");
            }
            else 
            {
                console.log("Response could not be converted correctly!");
            }
        })
        .catch((response) => {
            console.log(response.Message);
            return;
        });
    }


    return(
        <div className="res-background h-100">

            <div className='container-fluid d-flex justify-content-center h-25 login-container' id="loginCard">
                <div className='card login-card'>

                    <div className='login-card-header res-background p-2 border-bottom'>
                        <h3>Login Form</h3>
                    </div>


                    <div className='card-body res-background'>
                        <form onSubmit={loginFormSubmit}>

                            <div className='input-group form-group'>
                                <input type="text" name='username' className='form-control' placeholder='Username'></input>
                            </div>
                            <div className='input-group form-group'>
                                <input type="password" name='password' className='form-control' placeholder='Password'></input>
                            </div>
                            <div className='form-group justify-content-between mt-3'>
                                <button type="submit" className='btn btn-success'>Login</button>
                            </div>

                        </form>
                    </div>

                </div>
            </div>

            
            <div className="slide-container" id="slider">
                <Slide>

                    {SlideImages.map((slideImage, index)=> (
                        <div className="each-slide" key={index}>
                            <div className="slider-img" style={{'backgroundImage': `url(${slideImage.url})`}}>
                                <span>{slideImage.caption}</span>
                            </div>
                        </div>
                    ))} 

                </Slide>
            </div>

        </div>
    )
}

export function Logout({headtext}){
    const navigate = useNavigate();


    function Logout_uid(){
        //Submit the Uid to log out user, reset global variable and session storage
        //Reset headtext and move back to log in screen
        fetch(Base_user + "logoutuser", {
            method:"POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid
            })
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response);
            sessionStorage.clear();

            User.Uid = "";
            User.Details = {
                Id:0,
                Username:"",
                Password:"",
                Location:"",
                Permission:0,
                Active:0
            };

            headtext("");
            navigate("/login");
        })
        .catch((response) => {
            console.log(response.Message);
        });
    }

    return(
        <button className="button res-primary res-background m-2 float-right" onClick={Logout_uid}>
            Logout
        </button>
    );
}

