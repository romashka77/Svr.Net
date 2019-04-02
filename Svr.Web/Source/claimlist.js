import React from "react";
import ReactDOM from "react-dom";
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import reducer from './reducers';
//import claimsReducers from './Reducers/claims';
import WebPage from './Components/WebPage';

const store = createStore(
    //claimsReducers,
    reducer,
    window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
);

ReactDOM.render(
    <Provider store={store}>
        <WebPage />
    </Provider>,
    document.getElementById("fieldToShow")
);




//const addClimeBtn = $('.addClaim')[0];
//const idInput = $('.idInput')[0];
//const nameInput = $('.nameInput')[0];
//const list = $('.list')[0];

//store.subscribe(() => {
//    console.log('subscribe', store.getState());
    
//    list.innerHTML = '';
//    idInput.value = '';
//    nameInput.value = '';

//    store.getState().forEach((claim) => {
//        const li = document.createElement('li');
//        li.textContent =  claim.id+' '+ claim.name;
//        list.appendChild(li);
//    });

//    //store.getState().forEach((claim) => {
//    //    $('<li>', { textContent: claim.name }).appendTo('.list');
//    //});
//})
////const store = createStore(ClaimsReducers,
////    window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
////);




//addClimeBtn.addEventListener('click', () => {
//    const claimName = nameInput.value;
//    const claimId = idInput.value;
//    console.log('claimName', claimName);
//    console.log('claimId', claimId);
//    store.dispatch({ type: 'ADD_CLAIM', claim: { id: claimId, name: claimName } });
//});

module.hot.accept();