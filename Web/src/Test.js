import React, {useState, useEffect} from 'react'
import CryptoJS from 'react-native-crypto-js'

function Test() {
    const [data, setData] = useState("");
    
    useEffect(() =>{
        fetch("http://localhost:3000/loginuser?username=admin&password=" + CryptoJS.MD5("admin"), {
            method:"GET"
        })
        .then((res) => res.json())
        .then((response) =>{
            console.log(response);
            setData(response.Uid);
            })
        .catch(console.log);
    }, []);
    
    return (
    <div>
        A felhasználó UID-je:<br/>{data}
    </div>
    )
}

export default Test
