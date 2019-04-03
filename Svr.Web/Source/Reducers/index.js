import { combineReducers } from 'redux';
import claims from './claims';
import regions from './regions';
import filterClaims from './filterClaims';

export default combineReducers({
    claims,
    regions,
    filterClaims
});
