/* eslint-disable react/prop-types */
import { Helmet } from 'react-helmet-async'

const HelmetHandler = ({ title }) => {
    return (
        <Helmet>
            <title>{title} - Learnify</title>
        </Helmet>
    )
}

export default HelmetHandler
