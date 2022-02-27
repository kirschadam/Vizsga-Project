import React from 'react'
import { useState, useEffect } from 'react'
import { useNavigate} from "react-router-dom";
import CryptoJS from 'react-native-crypto-js';
import {User, Base_user} from './Variables';

export function Login() {
    const [isLoginPending, setLoginPending] = useState(false);
    const navigate = useNavigate();

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
        setLoginPending(true);
        fetch(Base_user + "loginuser?username=" + e.target.elements.username.value + "&password=" + CryptoJS.MD5(e.target.elements.password.value), {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            if(response.User.Active === 0){
                alert("This user is currently set to inactive!");
                return;
            }
            User.Uid = response.Uid;
            User.Details = response.User;
            if(User.Uid !== null && User.Uid !== ""){
                sessionStorage.setItem("Uid", response.Uid);
                sessionStorage.setItem("Details", JSON.stringify(response.User));

                document.getElementById("logout").style.visibility="visible";
                document.getElementById("navbar").style.visibility="visible";
                navigate("/products");
            }
            else {console.log("Response could not be converted correctly!");}
        })
        .catch((response) => {
            console.log(response.Message);
            return;
        });
        setLoginPending(false);
    }


    if (isLoginPending) {
        return(
            <div className='center-item'>
                <div className='spinner-border text-danger'></div>
            </div>

        );
    }

    return(
        <div className='container-fluid d-flex justify-content-center h-25 login-container' id="loginCard">
            <div className='card login-card'>
                <div className='res-background p-2 border-bottom login-card-header'>
                <h3>Login Form</h3>
                </div>

                <div className='card-body res-background'>
                    <form onSubmit={loginFormSubmit}>
                        <div className='input-group form-group m-2'>
                            <input type="text" name='username' className='form-control' placeholder='Username'></input>
                        </div>
                        <div className='input-group form-group m-2'>
                            <input type="password" name='password' className='form-control' placeholder='Password'></input>
                        </div>
                        <div className='form-group m-2'>
                            <button type="submit" className='btn btn-success'>Login</button>
                        </div>
                    </form>
                </div>

            </div>

        </div>
    )
}

export function Logout(){
    const navigate = useNavigate();

    function Logout_uid(){
        fetch(Base_user + "logoutuser?uid=" + User.Uid, {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
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
            navigate("/login");
        })
        .catch((response) => {
            console.log(response.Message);
        });
    }

    return(
        <button
            className="btn btn-danger m-2 float-right"
            onClick={Logout_uid}>Logout</button>
    );
}

