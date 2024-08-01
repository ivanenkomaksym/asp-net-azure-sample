import { useState, useEffect } from 'react';
import featureFlags from './authConfig.js';

const useFeatureFlags = () => {
  const [features, setFeatures] = useState({});

  useEffect(() => {
    setFeatures(featureFlags);
  }, []);

  const isFeatureEnabled = (feature) => features[feature];

  return { isFeatureEnabled };
};

const isSelectOrganizationEnabled = () => {
    const { isFeatureEnabled } = useFeatureFlags();
    return isFeatureEnabled('selectOrganization');
}

export default isSelectOrganizationEnabled;