import React from 'react';
import { Container } from 'reactstrap';
import NavMenu from '../nav-menu';

export default props => (
    <div>
        <NavMenu />
        <Container>
            {props.children}
        </Container>
    </div>
);
