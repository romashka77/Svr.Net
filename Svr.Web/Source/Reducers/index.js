import { combineReducers } from 'redux';
import ClaimsReducers from './claim';

const allReducers = combineReducers({
    claims: ClaimsReducers
});

export default allReducers;