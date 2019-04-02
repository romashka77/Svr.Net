import React from "react";
import ReactDOM from "react-dom";
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import { allReducers } from './Reducers';
//import ClaimsReducers from './Reducers/claim';
import WebPage from './Components/WebPage';

const store = createStore(allReducers);
//const store = createStore(ClaimsReducers);


ReactDOM.render(
    <Provider store={store}>
        <WebPage />
    </Provider>,
    document.getElementById("fieldToShow")
);



//module.hot.accept();