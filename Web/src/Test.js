import React, {useState, useEffect} from 'react';
import {User} from './Variables'

function Test() {
    const [data, setData] = useState("");
    
    useEffect(() =>{
        console.log(User.Uid);
        setData(User.Uid);
    }, []);
    
    return (
    <div>
        A felhasználó UID-je:<br/>{data}
    </div>
    )
}

export default Test
