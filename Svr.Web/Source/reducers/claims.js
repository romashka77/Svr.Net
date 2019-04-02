const initialState = [
    {
        id: 1,
        name: 'test1'
    },
    {
        id: 2,
        name: 'test2'
    },
    {
        id: 3,
        name: 'test4'
    }
];


export default function claims(state = initialState, action) {
    console.log(action);
    if (action.type === 'ADD_CLAIM') {
        return [
            ...state,
            action.claim
        ];
    } else if (action.type === 'DELETE_CLAIM') {
        return state;
    }
    return state;
}