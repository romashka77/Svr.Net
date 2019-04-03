const initialState = [];


export default function claims(state = initialState, action) {
    if (action.type === 'ADD_CLAIM') {
        return [
            ...state,
            action.payload
        ];
    } else if (action.type === 'FETCH_CLAIMS_SUCCESS') {
        return action.payload;
    }
    return state;
}