import { GET_POSTS_SUCCESS, GET_POSTS_ERROR } from './blogConstants.jsx';
import "isomorphic-fetch";

export function receivePosts(data) {
    return {
        type: GET_POSTS_SUCCESS,
        posts: data
    };
}

export function errorReceive(err) {
    return {
        type: GET_POSTS_ERROR,
        error: err
    };
}

export function getPosts(pageIndex = 0, tag) {
    return (dispatch) => {
        let queryTrailer = '?pageIndex=' + pageIndex;
        if (tag) {
            queryTrailer += '&tag=' + tag;
        }
        fetch(constants.getPage + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receivePosts(data));
            }).catch((ex) => {
                dispatch(errorReceive(err));
            });
    };
}