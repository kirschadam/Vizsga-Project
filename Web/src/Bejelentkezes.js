import React from 'react'
import { useState } from 'react'
import { useNavigate} from "react-router-dom";
import CryptoJS from 'react-native-crypto-js';
import {User, Base_user} from './Variables';

export function Bejelentkezes() {
    const [isLoginPending, setLoginPending] = useState(false);
    const navigate = useNavigate();

    function loginFormSubmit(e) {
        e.preventDefault();
        setLoginPending(true);
        fetch(Base_user + "loginuser?username=" + e.target.elements.username.value + "&password=" + CryptoJS.MD5(e.target.elements.password.value), {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            User.Uid = response.Uid;
            User.Details = response.User;
            navigate("/test");
        })
        .catch((response) => {
            console.log(response.Message);
            navigate("/bejelentkezes");
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
        <div className='container-fluid d-flex justify-content-center h-100 login-container'>
            <div className='card login-card'>
                <div className='card-header login-card-header'>
                <h3>Bejelentkezés</h3>
                </div>

                <div className='card-body'>
                    <form onSubmit={loginFormSubmit}>
                        <div className='input-group form-group'>
                            <input type="text" name='username' className='form-control' placeholder='Username'></input>
                        </div>
                        <div className='input-group form-group'>
                            <input type="password" name='password' className='form-control' placeholder='Password'></input>
                        </div>
                        <div className='form-group'>
                            <button type="submit" className='btn float-right btn-warning'>Küldés</button>
                        </div>
                    </form>
                </div>

            </div>

        </div>
    )
}

